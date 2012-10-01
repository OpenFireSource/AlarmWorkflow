using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using System.Xml;

namespace AlarmWorkflow.Shared
{
    /// <summary>
    /// This class is started in a own thread, and do all that work.
    /// </summary>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    internal sealed class WorkingThread : IDisposable
    {
        #region Fields

        private FileSystemWatcher fileSystemWatcher;

        private DirectoryInfo _faxPath;
        private DirectoryInfo _archivePath;
        private DirectoryInfo _analysisPath;

        private Dictionary<string, string> _replaceDictionary;

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
        public WorkingThread()
        {
            Jobs = new List<IJob>();
            UseOCRSoftware = OcrSoftware.Cuneiform;
            OcrPath = String.Empty;

            InitializeReplaceDictionary();
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

            this.fileSystemWatcher = new FileSystemWatcher(_faxPath.FullName, "*.TIF");
            this.fileSystemWatcher.IncludeSubdirectories = false;
            this.fileSystemWatcher.Created += new FileSystemEventHandler(_fileSystemWatcher_Created);
            this.fileSystemWatcher.WaitForChanged(WatcherChangeTypes.Created);
            this.fileSystemWatcher.EnableRaisingEvents = true;
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
                this.fileSystemWatcher.Dispose();
                this.fileSystemWatcher = null;
            }
        }

        #endregion

        #region Event handlers

        // TODO: This Routine is dangerous! If there is for whatever reason a delay in processing, and multiple faxes are incoming, they go unprocessed due to the fact...
        // ... that the routine is busy processing the first fax! Change this to a loop which runs every three seconds or so (more than sufficient)!
        // TODO: When debugging this routine is very unreliable! Sometimes it won't get called at all???!??!?!
        private void _fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            this.fileSystemWatcher.EnableRaisingEvents = false;
            FileInfo f;
            try
            {
                f = new FileInfo(e.FullPath);
            }
            catch (ArgumentNullException ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Error while ceating File Info Object for new Fax: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (SecurityException ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Error while ceating File Info Object for new Fax: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (ArgumentException ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Error while ceating File Info Object for new Fax: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Error while ceating File Info Object for new Fax: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (PathTooLongException ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Error while ceating File Info Object for new Fax: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (NotSupportedException ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Error while ceating File Info Object for new Fax: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }

            string analyseFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            bool fileIsMoved = false;
            int tried = 0;
            while (!fileIsMoved)
            {
                tried++;
                try
                {
                    f.MoveTo(Path.Combine(_archivePath.FullName, analyseFileName + ".TIF"));
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
                        this.fileSystemWatcher.EnableRaisingEvents = true;
                        return;
                    }
                }
            }

            try
            {
                using (Image img = Image.FromFile(Path.Combine(_archivePath.FullName, analyseFileName + ".TIF")))
                {
                    // TODO: This will only work with cuneiform (bmp). Tesseract needs TIF!
                    img.Save(Path.Combine(_archivePath.FullName, analyseFileName + ".bmp"), System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
            catch (OutOfMemoryException ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Error while reading tif image: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (FileNotFoundException ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Error while reading tif image: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (ArgumentException ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Error while reading tif image: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }

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
                            proc.StartInfo.Arguments = f.DirectoryName + "\\" + analyseFileName + ".bmp " + _analysisPath.FullName + analyseFileName + " -l deu";
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
                            proc.StartInfo.Arguments = @"-l ger --singlecolumn -o " + _analysisPath.FullName + analyseFileName + ".txt " + f.DirectoryName + @"\" + analyseFileName + ".bmp";
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
                    this.fileSystemWatcher.EnableRaisingEvents = true;
                    return;
                }

                Operation operation = null;
                try
                {
                    // Read the analysis file...
                    using (StreamReader reader = new StreamReader(Path.Combine(_analysisPath.FullName, analyseFileName + ".txt")))
                    {
                        // ... fetch all lines ...
                        List<string> parsedLines = new List<string>();
                        foreach (string preParsedLine in reader.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                        {
                            string tmp = preParsedLine;
                            foreach (var pair in _replaceDictionary)
                            {
                                tmp = tmp.Replace(pair.Key, pair.Value);
                            }
                            parsedLines.Add(tmp);
                        }

                        // Try to parse the operation. If parsing failed, ignore this but write to the log file!
                        operation = Parser.Parse(parsedLines.ToArray());
                    }

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
                            Logger.Instance.LogFormat(LogType.Warning, this, string.Format("An error occurred while processing job '{0}'. The error message was: {1}", job.GetType().Name, ex.Message));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, "An exception occurred while parsing the alarmfax! The error message was: " + ex.Message);
                }

                this.fileSystemWatcher.EnableRaisingEvents = true;
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
