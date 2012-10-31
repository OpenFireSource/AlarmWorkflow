using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.AlarmSource.Fax
{
    /// <summary>
    /// Implements the <see cref="IAlarmSource"/>-interface to provide an alarm source that handles incoming faxes.
    /// </summary>
    [Export("FaxAlarmSource", typeof(IAlarmSource))]
    sealed class FaxAlarmSource : IAlarmSource
    {
        #region Fields

        private FaxConfiguration _configuration;

        private DirectoryInfo _faxPath;
        private DirectoryInfo _archivePath;
        private DirectoryInfo _analysisPath;

        private DirectoryInfo _ocrPath;
        private IParser _parser;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FaxAlarmSource"/> class.
        /// </summary>
        public FaxAlarmSource()
        {
            _configuration = new FaxConfiguration();

            InitializeSettings();
        }

        #endregion

        #region Methods

        private void InitializeSettings()
        {
            _faxPath = new DirectoryInfo(_configuration.FaxPath);
            _archivePath = new DirectoryInfo(_configuration.ArchivePath);
            _analysisPath = new DirectoryInfo(_configuration.AnalysisPath);

            string ocrPath = null;
            if (_configuration.OCRSoftware == OcrSoftware.Tesseract)
            {
                if (string.IsNullOrWhiteSpace(_configuration.OCRSoftwarePath)) { ocrPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\tesseract"; }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_configuration.OCRSoftwarePath)) { ocrPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\cuneiform"; }
            }
            _ocrPath = new DirectoryInfo(ocrPath);
            if (!_ocrPath.Exists)
            {
                throw new DirectoryNotFoundException(string.Format("The OCR software '{0}' was suggested to be found in path '{1}', which doesn't exist!", _configuration.OCRSoftware, _ocrPath.FullName));
            }

            // Import parser with the given name/alias
            _parser = ExportedTypeLibrary.Import<IParser>(_configuration.AlarmFaxParserAlias);
            Logger.Instance.LogFormat(LogType.Info, this, "Using parser '{0}'.", _parser.GetType().FullName);
        }

        /// <summary>
        /// Makes sure that the required directories exist and we don't run into unnecessary exceptions.
        /// </summary>
        private void EnsureDirectoriesExist()
        {
            try
            {
                if (!_faxPath.Exists)
                {
                    _faxPath.Create();
                }
                if (!_archivePath.Exists)
                {
                    _archivePath.Create();
                }
                if (!_analysisPath.Exists)
                {
                    _analysisPath.Create();
                }
            }
            catch (IOException)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Could not create any of the default directories. Try running the process as Administrator, or create the directories in advance.");
            }
        }

        private void ProcessFile(FileInfo file)
        {
            string analyseFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            bool fileIsMoved = false;
            int tried = 0;
            while (!fileIsMoved)
            {
                tried++;
                try
                {
                    file.MoveTo(Path.Combine(_archivePath.FullName, analyseFileName + ".TIF"));
                    fileIsMoved = true;
                }
                catch (IOException ex)
                {
                    if (tried < 60)
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this, "Coudn't move file. Try {0} of 10!", tried);
                        Thread.Sleep(200);
                        fileIsMoved = false;
                    }
                    else
                    {
                        Logger.Instance.LogFormat(LogType.Error, this, "Coundn't move file.\n" + ex.ToString());
                        return;
                    }
                }
            }

            // One catch of TIFF is that it may contain multiple pages! This is by itself no problem, but the OCR software
            // may not be able to recognize this. So we do the following:
            // 1. We read the TIFF, and if it is a multipage-tiff-file...
            // 2. ... we will split each page into an own file (done in the method) ...
            // 3. ... and THEN we will scan each "page" and concat them together, so it appears to the parser as one file.
            List<string> analyzedLines = new List<string>();
            foreach (string imageFile in Utilities.GetMergedTifFileNames(Path.Combine(_archivePath.FullName, analyseFileName + ".TIF")))
            {
                string intendedNewFileName = Path.Combine(_analysisPath.FullName, Path.GetFileNameWithoutExtension(imageFile) + ".txt");

                // Host the configured OCR-software in a new process and run it
                using (Process proc = new Process())
                {
                    proc.EnableRaisingEvents = false;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.WorkingDirectory = _ocrPath.FullName;

                    switch (_configuration.OCRSoftware)
                    {
                        case OcrSoftware.Tesseract:
                            {
                                proc.StartInfo.FileName = Path.Combine(proc.StartInfo.WorkingDirectory, "tesseract.exe");
                                proc.StartInfo.Arguments = file.DirectoryName + "\\" + analyseFileName + ".bmp " + intendedNewFileName + " -l deu";

                                // Correct txt path for tesseract (it will append .txt under windows always)
                                intendedNewFileName += ".txt";
                            }
                            break;
                        case OcrSoftware.Cuneiform:
                        default:
                            {
                                proc.StartInfo.FileName = Path.Combine(proc.StartInfo.WorkingDirectory, "cuneiform.exe");
                                proc.StartInfo.Arguments = "-l ger --singlecolumn -o " + intendedNewFileName + " " + imageFile;
                            }
                            break;
                    }

                    try
                    {
                        proc.Start();
                        proc.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this, "Error while the ocr Prozess: " + ex.ToString());
                        return;
                    }

                    // After the file has been parsed, read it back in ...
                    // ... fetch all lines ...
                    foreach (string preParsedLine in File.ReadAllLines(intendedNewFileName))
                    {
                        // ... and add it to the list (
                        analyzedLines.Add(_configuration.PerformReplace(preParsedLine));
                    }
                }
            }

            Operation operation = null;
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                // Try to parse the operation. If parsing failed, ignore this but write to the log file!
                Logger.Instance.LogFormat(LogType.Trace, this, "Begin parsing incoming operation...");

                string[] lines = analyzedLines.ToArray();
                // Find out if the fax is a test-fax
                if (IsTestFax(lines))
                {
                    Logger.Instance.LogFormat(LogType.Trace, this, "Operation is a test-fax. Parsing is skipped.");
                }
                else
                {
                    operation = _parser.Parse(lines);
                }

                sw.Stop();
                Logger.Instance.LogFormat(LogType.Trace, this, "Parsed operation in '{0}' milliseconds.", sw.ElapsedMilliseconds);

                // If there is no timestamp, use the current time. Not too good but better than MinValue :-/
                if (operation.Timestamp == DateTime.MinValue)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, "Could not parse timestamp from the fax. Using the current time as the timestamp.");
                    operation.Timestamp = DateTime.Now;
                }

                // Raise event...
                OnNewAlarm(operation);

            }
            catch (Exception ex)
            {
                sw.Stop();
                Logger.Instance.LogFormat(LogType.Warning, this, "An exception occurred while processing the alarmfax!");
                Logger.Instance.LogException(this, ex);
            }
        }

        // Checks the raw line contents for any occurrences of test-fax keywords.
        private bool IsTestFax(string[] lines)
        {
            return lines.Any(l => _configuration.TestFaxKeywords.Any(kw => l.Contains(kw)));
        }

        #endregion

        #region IAlarmSource Members

        /// <summary>
        /// Raised when a new alarm has surfaced and processed for the Engine to handle.
        /// See documentation for further information.
        /// </summary>
        public event EventHandler<AlarmSourceEventArgs> NewAlarm;

        private void OnNewAlarm(Operation operation)
        {
            var copy = NewAlarm;
            if (copy != null)
            {
                copy(this, new AlarmSourceEventArgs(operation));
            }
        }

        void IAlarmSource.Initialize()
        {

        }

        void IAlarmSource.RunThread()
        {
            EnsureDirectoriesExist();

            while (true)
            {
                FileInfo[] files = _faxPath.GetFiles("*.tif", SearchOption.TopDirectoryOnly);
                if (files.Length > 0)
                {
                    Logger.Instance.LogFormat(LogType.Trace, this, "Processing '{0}' new faxes...", files.Length);

                    foreach (FileInfo file in files)
                    {
                        ProcessFile(file);
                    }

                    Logger.Instance.LogFormat(LogType.Trace, this, "Processing finished.");
                }
                Thread.Sleep(_configuration.RoutineInterval);
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {

        }

        #endregion
    }
}
