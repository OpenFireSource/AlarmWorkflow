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
        private const string AnalyzedFileNameFormat = "yyyyMMddHHmmssffff";
        private const string ArchivedFilePathExtension = ".tif";
        private const int MoveFileAttemptDelayMs = 200;
        private const int RoutineIntervalMs = 2000;

        #endregion

        #region Fields

        private FaxConfiguration _configuration;

        private DirectoryInfo _faxPath;
        private DirectoryInfo _archivePath;
        private DirectoryInfo _analysisPath;

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

        private void ProcessNewImage(FileInfo file)
        {
            EnsureDirectoriesExist();

            string analyseFileName = DateTime.Now.ToString(AnalyzedFileNameFormat);
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

                if (!IsOnWhitelist(lines))
                {
                    Logger.Instance.LogFormat(LogType.Info, this, Properties.Resources.FaxIsNotOnWhitelist);
                    return;
                }

                if (IsOnBlacklist(lines))
                {
                    Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.FaxIsOnBlacklist);
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

        private bool IsOnBlacklist(string[] lines)
        {
            return lines.Any(l => _configuration.FaxBlacklist.Any(kw => l.Contains(kw)));
        }

        private bool IsOnWhitelist(string[] lines)
        {
            IEnumerable<string> whitelist = _configuration.FaxWhitelist;
            if (!whitelist.Any())
            {
                return true;
            }

            return lines.Any(l => whitelist.Any(kw => l.Contains(kw)));
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

            _faxPath = new DirectoryInfo(_configuration.FaxPath);
            _archivePath = new DirectoryInfo(_configuration.ArchivePath);
            _analysisPath = new DirectoryInfo(_configuration.AnalysisPath);

            InitializeOcrSoftware();
            InitializeParser();
        }

        private void InitializeParser()
        {
            _parser = ExportedTypeLibrary.Import<IParser>(_configuration.AlarmFaxParserAlias);
            Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.UsingParserTrace, _parser.GetType().FullName);
        }

        void IAlarmSource.RunThread()
        {
            Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.UsingIncomingFaxDirectory, _faxPath.FullName);
            Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.UsingAnalyzedFaxDirectory, _analysisPath.FullName);
            Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.UsingArchivedFaxDirectory, _archivePath.FullName);

            EnsureDirectoriesExist();

            while (true)
            {
                FileInfo[] files = _faxPath.GetFiles("*.tif", SearchOption.TopDirectoryOnly);
                if (files.Length > 0)
                {
                    Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.BeginProcessingFaxes, files.Length);

                    foreach (FileInfo file in files)
                    {
                        ProcessNewImage(file);
                    }

                    Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.ProcessingFaxesComplete);
                }

                Thread.Sleep(RoutineIntervalMs);
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            _configuration.Dispose();
            _configuration = null;
        }

        #endregion
    }
}