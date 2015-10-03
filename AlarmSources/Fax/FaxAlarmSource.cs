// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using AlarmWorkflow.AlarmSource.Fax.Extensibility;
using AlarmWorkflow.AlarmSource.Fax.OcrSoftware;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Specialized;
using Ghostscript.NET;

namespace AlarmWorkflow.AlarmSource.Fax
{
    /// <summary>
    /// Implements the <see cref="IAlarmSource"/>-interface to provide an alarm source that handles incoming faxes.
    /// </summary>
    [Export("FaxAlarmSource", typeof(IAlarmSource))]
    [Information(DisplayName = "ExportAlarmSourceDisplayName", Description = "ExportAlarmSourceDescription")]
    sealed class FaxAlarmSource : IAlarmSource
    {
        #region Constants

        private const int ErrorRetryCount = 10;
        private const string ArchivedFilePathExtension = ".tif";
        private const int MoveFileAttemptDelayMs = 200;
        private const int RoutineIntervalMs = 2000;

        #endregion

        #region Fields

        private IServiceProvider _serviceProvider;
        private FaxConfiguration _configuration;

        private DirectoryInfo _faxPath;
        private DirectoryInfo _archivePath;
        private DirectoryInfo _analysisPath;

        private string _analyzedFileNameFormat;

        private IOcrSoftware _ocrSoftware;
        private IParser _parser;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FaxAlarmSource"/> class.
        /// </summary>
        public FaxAlarmSource()
        {
        }

        #endregion

        #region Methods

        private void InitializeOcrSoftware()
        {
            AssertCustomOcrPathExist();

            /* Hard-coded to internally used tesseract.
             */
            _ocrSoftware = new TesseractOcrSoftware();
            Logger.Instance.LogFormat(LogType.Info, this, Properties.Resources.InitializeUsingOcrSoftware, _ocrSoftware.GetType().Name);
        }

        private void AssertCustomOcrPathExist()
        {
            if (string.IsNullOrWhiteSpace(_configuration.OCRSoftwarePath))
            {
                return;
            }

            if (Directory.Exists(_configuration.OCRSoftwarePath))
            {
                return;
            }

            throw new DirectoryNotFoundException(string.Format(Properties.Resources.OcrSoftwareNotFoundError, _ocrSoftware.GetType().Name, _configuration.OCRSoftwarePath));
        }

        /// <summary>
        /// Makes sure that the required directories exist and we don't run into unnecessary exceptions.
        /// </summary>
        private void EnsureDirectoriesExist()
        {
            try
            {
                _faxPath.Refresh();
                _archivePath.Refresh();
                _analysisPath.Refresh();

                if (!_faxPath.Exists)
                {
                    _faxPath.Create();
                    Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.CreatedRequiredDirectory, _faxPath.FullName);
                }
                if (!_archivePath.Exists)
                {
                    Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.CreatedRequiredDirectory, _archivePath.FullName);
                    _archivePath.Create();
                }
                if (!_analysisPath.Exists)
                {
                    Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.CreatedRequiredDirectory, _analysisPath.FullName);
                    _analysisPath.Create();
                }
            }
            catch (IOException)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.ErrorCreatingRequiredDirectory);
            }
        }

        private void ProcessNewPdf(FileInfo file)
        {
            string tiffFilePath = Path.Combine(file.DirectoryName, "convert.tif");
            Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.GhostscriptConvert, file.FullName);

            try
            {
                GhostscriptImageDevice dev = new GhostscriptImageDevice();
                dev.Device = "tiffgray";
                dev.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
                dev.TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
                dev.Resolution = 300;
                dev.InputFiles.Add(file.FullName);
                dev.OutputPath = tiffFilePath;
                dev.Process();
                
                ProcessNewImage(new FileInfo(tiffFilePath));
                
                file.Delete();
            }
            catch (GhostscriptException ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.GhostscriptConvertError);
                Logger.Instance.LogException(this, ex);

                string archivedFilePath = Path.Combine(_archivePath.FullName, file.Name);
                MoveFileTo(file, archivedFilePath);
            }
        }

        private void ProcessNewImage(FileInfo file)
        {
            EnsureDirectoriesExist();

            string analyseFileName = DateTime.Now.ToString(_analyzedFileNameFormat);
            string archivedFilePath = Path.Combine(_archivePath.FullName, analyseFileName + ArchivedFilePathExtension);

            MoveFileTo(file, archivedFilePath);

            string[] parsedLines = null;
            try
            {
                OcrProcessOptions options = new OcrProcessOptions();
                options.SoftwarePath = _configuration.OCRSoftwarePath;
                options.AnalyzedFileDestinationPath = Path.Combine(_analysisPath.FullName, Path.GetFileNameWithoutExtension(file.FullName));
                options.ImagePath = file.FullName;

                Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.OcrSoftwareParseBegin, file.FullName);

                Stopwatch swParse = Stopwatch.StartNew();

                parsedLines = _ocrSoftware.ProcessImage(options);

                swParse.Stop();

                Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.OcrSoftwareParseEndSuccess, swParse.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.OcrSoftwareParseEndFail);
                Logger.Instance.LogException(this, ex);
                return;
            }

            IList<string> analyzedLines = new List<string>();
            ReplaceDictionary replDict = _configuration.ReplaceDictionary;

            foreach (string preParsedLine in parsedLines)
            {
                analyzedLines.Add(replDict.ReplaceInString(preParsedLine));
            }

            Operation operation = null;
            try
            {
                Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.BeginParsingIncomingOperation);

                string[] lines = analyzedLines.ToArray();

                if (!_serviceProvider.GetService<IAlarmFilter>().QueryAcceptSource(string.Join(" ", lines)))
                {
                    return;
                }

                Stopwatch sw = Stopwatch.StartNew();

                operation = _parser.Parse(lines);

                sw.Stop();
                Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.ParsingOperationCompleted, sw.ElapsedMilliseconds);

                // If there is no timestamp, use the current time. Not too good but better than MinValue :-/
                if (operation.Timestamp == DateTime.MinValue)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.ParsingTimestampFailedUsingCurrentTime);
                    operation.Timestamp = DateTime.Now;
                }

                IDictionary<string, object> ctxParameters = new Dictionary<string, object>();
                ctxParameters[ContextParameterKeys.ArchivedFilePath] = archivedFilePath;
                ctxParameters[ContextParameterKeys.ImagePath] = file.FullName;

                AlarmSourceEventArgs args = new AlarmSourceEventArgs(operation);
                args.Parameters = ctxParameters;

                OnNewAlarm(args);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.ProcessNewImageError);
                Logger.Instance.LogException(this, ex);
            }
        }

        private void MoveFileTo(FileInfo file, string archivedFilePath)
        {
            bool fileIsMoved = false;
            int attemptNr = 0;

            while (!fileIsMoved)
            {
                attemptNr++;
                try
                {
                    file.MoveTo(archivedFilePath);
                    fileIsMoved = true;
                }
                catch (IOException ex)
                {
                    if (attemptNr < ErrorRetryCount)
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.MoveFileAttemptError, attemptNr, ErrorRetryCount);

                        Thread.Sleep(MoveFileAttemptDelayMs);
                    }
                    else
                    {
                        Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.MoveFileFailure);
                        Logger.Instance.LogException(this, ex);
                    }
                }
            }
        }

        private void InitializeFaxPaths()
        {
            _faxPath = new DirectoryInfo(_configuration.FaxPath);
            _archivePath = new DirectoryInfo(_configuration.ArchivePath);
            _analysisPath = new DirectoryInfo(_configuration.AnalysisPath);

            _analyzedFileNameFormat = _configuration.AnalyzedFileNameFormat;

            Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.UsingIncomingFaxDirectory, _faxPath.FullName);
            Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.UsingAnalyzedFaxDirectory, _analysisPath.FullName);
            Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.UsingArchivedFaxDirectory, _archivePath.FullName);

            EnsureDirectoriesExist();
        }

        private void InitializeParser()
        {
            _parser = ExportedTypeLibrary.Import<IParser>(_configuration.AlarmFaxParserAlias);
            Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.UsingParserTrace, _parser.GetType().FullName);
        }

        #endregion

        #region IAlarmSource Members

        /// <summary>
        /// Raised when a new alarm has surfaced and processed for the Engine to handle.
        /// See documentation for further information.
        /// </summary>
        public event EventHandler<AlarmSourceEventArgs> NewAlarm;

        private void OnNewAlarm(AlarmSourceEventArgs args)
        {
            var copy = NewAlarm;
            if (copy != null)
            {
                copy(this, args);
            }
        }

        void IAlarmSource.Initialize(IServiceProvider serviceProvider)
        {
            _configuration = new FaxConfiguration(serviceProvider);
            _configuration.PropertyChanged += _configuration_PropertyChanged;

            _serviceProvider = serviceProvider;

            InitializeFaxPaths();
            InitializeOcrSoftware();
            InitializeParser();
        }

        void IAlarmSource.RunThread()
        {
            while (true)
            {
                //.tif or .pdf
                FileInfo[] files = _faxPath.GetFiles("*.*", SearchOption.TopDirectoryOnly)
                            .Where(_ => _.Name.EndsWith(".tif") || _.Name.EndsWith(".pdf"))
                            .ToArray();

                if (files.Length > 0)
                {
                    Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.BeginProcessingFaxes, files.Length);

                    foreach (FileInfo file in files)
                    {
                        if(file.Extension == ".pdf")
                        {
                            ProcessNewPdf(file);
                        }
                        else
                        {
                            ProcessNewImage(file);
                        }
                    }

                    Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.ProcessingFaxesComplete);
                }

                Thread.Sleep(RoutineIntervalMs);
            }
        }

        #endregion

        #region Event-Handler

        private void _configuration_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "AlarmfaxParser":
                    InitializeParser();
                    break;
                case "OCR.Path":
                    AssertCustomOcrPathExist();
                    break;
                case "FaxPaths":
                    InitializeFaxPaths();
                    break;
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            _configuration.PropertyChanged -= _configuration_PropertyChanged;
            _configuration.Dispose();
            _configuration = null;
        }

        #endregion
    }
}