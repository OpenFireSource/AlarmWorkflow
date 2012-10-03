using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Shared
{
    /// <summary>
    /// This class is started in a own thread, and do all that work.
    /// </summary>
    internal sealed class WorkingThread : IDisposable
    {
        #region Fields

        private DirectoryInfo _faxPath;
        private DirectoryInfo _archivePath;
        private DirectoryInfo _analysisPath;

        private Dictionary<string, string> _replaceDictionary;

        private IOperationStore _operationStore;

        #endregion

        #region Properties

        /// <value>
        /// Gets the job list.
        /// </value>
        /// <summary>
        /// Gets the job list.
        /// </summary>
        internal List<IJob> Jobs { get; private set; }

        /// <summary>
        /// Sets the fax path.
        /// </summary>
        internal string FaxPath
        {
            set { _faxPath = new DirectoryInfo(value); }
        }
        /// <summary>
        /// Sets the archiev path.
        /// </summary>
        internal string ArchivePath
        {
            set { _archivePath = new DirectoryInfo(value); }
        }
        /// <summary>
        /// Sets the analysis path.
        /// </summary>
        internal string AnalysisPath
        {
            set { _analysisPath = new DirectoryInfo(value); }
        }
        /// <summary>
        /// Gets/sets the useOCRSoftware.
        /// </summary>
        internal OcrSoftware UseOCRSoftware { get; set; }
        /// <summary>
        /// Gets/sets the useOCRSoftware.
        /// </summary>
        internal string OcrPath { get; set; }
        /// <summary>
        /// Gets/sets the parser to be used.
        /// </summary>
        internal IParser Parser { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkingThread"/> class.
        /// </summary>
        /// <param name="parent"></param>
        public WorkingThread(AlarmworkflowClass parent)
        {
            Jobs = new List<IJob>();
            UseOCRSoftware = OcrSoftware.Cuneiform;
            OcrPath = String.Empty;

            InitializeReplaceDictionary();

            _operationStore = parent.GetOperationStore();
        }

        #endregion

        #region Methods

        private void InitializeReplaceDictionary()
        {
            _replaceDictionary = new Dictionary<string, string>(16);

            XmlDocument doc = new XmlDocument();
            doc.Load(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Config\ReplaceDictionary.xml");

            XmlNode replacingNode = doc.GetElementsByTagName("Dictionary")[0];
            foreach (XmlNode rpn in replacingNode.ChildNodes)
            {
                _replaceDictionary[rpn.Attributes["Old"].InnerText] = rpn.Attributes["New"].InnerText;
            }
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

        /// <summary>
        /// This Methode is started with the thread start.
        /// </summary>
        internal void DoWork()
        {
            EnsureDirectoriesExist();

            bool run = true;
            while (run)
            {
                FileInfo[] files = _faxPath.GetFiles("*.tif", SearchOption.TopDirectoryOnly);
                if (files.Length > 0)
                {
                    foreach (FileInfo file in files)
                    {
                        ProcessFile(file);
                    }
                }
                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// Clean the object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clean the object.
        /// </summary>
        /// <param name="alsoManaged">Indicates if also managed code shoud be cleaned up.</param>
        private void Dispose(bool alsoManaged)
        {
            if (alsoManaged == true)
            {

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

                    switch (UseOCRSoftware)
                    {
                        case OcrSoftware.Tesseract:
                            {
                                if (String.IsNullOrEmpty(OcrPath))
                                {
                                    proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\tesseract";
                                }
                                else
                                {
                                    proc.StartInfo.WorkingDirectory = OcrPath;
                                }

                                proc.StartInfo.FileName = Path.Combine(proc.StartInfo.WorkingDirectory, "tesseract.exe");
                                proc.StartInfo.Arguments = file.DirectoryName + "\\" + analyseFileName + ".bmp " + intendedNewFileName + " -l deu";

                                // Correct txt path for tesseract (it will append .txt under windows always)
                                intendedNewFileName += ".txt";
                            }
                            break;
                        case OcrSoftware.Cuneiform:
                        default:
                            {
                                if (String.IsNullOrEmpty(OcrPath))
                                {
                                    proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\cuneiform";
                                }
                                else
                                {
                                    proc.StartInfo.WorkingDirectory = OcrPath;
                                }

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
                        string tmp = preParsedLine;
                        foreach (var pair in _replaceDictionary)
                        {
                            tmp = tmp.Replace(pair.Key, pair.Value);
                        }
                        // ... and add it to the list
                        analyzedLines.Add(tmp);
                    }
                }
            }

            Operation operation = null;
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                // Try to parse the operation. If parsing failed, ignore this but write to the log file!
                Logger.Instance.LogFormat(LogType.Info, this, "Begin parsing incoming operation...");

                operation = Parser.Parse(analyzedLines.ToArray());

                sw.Stop();
                Logger.Instance.LogFormat(LogType.Debug, this, "Parsed operation in '{0}' milliseconds.", sw.ElapsedMilliseconds);

                // Grant the operation a new Id
                operation.Id = _operationStore.GetNextOperationId();

                foreach (IJob job in Jobs)
                {
                    try
                    {
                        // Run the job. If the job fails, ignore that exception as well but log it too!
                        if (!job.DoJob(operation))
                        {
                            Logger.Instance.LogFormat(LogType.Warning, this, job.ErrorMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Be careful when processing the jobs, we don't want a malicious job to terminate the process!
                        Logger.Instance.LogFormat(LogType.Warning, this, string.Format("An error occurred while processing job '{0}'!", job.GetType().Name));
                        Logger.Instance.LogException(this, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "An exception occurred while processing the alarmfax!");
                Logger.Instance.LogException(this, ex);
            }
        }

        #endregion
    }

    /// <summary>
    /// List all availeble OCRSoftware.
    /// </summary>
    public enum OcrSoftware
    {
        /// <summary>
        /// Tesseract OCR Software from http://code.google.com/p/tesseract-ocr/.
        /// </summary>
        Tesseract,
        /// <summary>
        /// Cuneiform for Linux. Mit Anpassungen für singlecolumn https://launchpad.net/cuneiform-linux.
        /// </summary>
        Cuneiform
    }

}
