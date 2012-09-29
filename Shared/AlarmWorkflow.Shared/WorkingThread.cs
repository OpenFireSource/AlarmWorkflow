using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Logging;

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
        /// Gets or sets the logger object.
        /// </summary>
        internal ILogger Logger { get; set; }
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
        /// Gets/sets the replacing list.
        /// </summary>
        internal List<ReplaceString> ReplacingList { get; set; }
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
        }

        #endregion

        #region Methods

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
                this.Logger.WriteError("Could not create any of the default directories. Try running the process as Administrator, or create the directories in advance.");
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
                this.Logger.WriteError("Error while ceating File Info Object for new Fax: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (SecurityException ex)
            {
                this.Logger.WriteError("Error while ceating File Info Object for new Fax: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (ArgumentException ex)
            {
                this.Logger.WriteError("Error while ceating File Info Object for new Fax: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                this.Logger.WriteError("Error while ceating File Info Object for new Fax: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (PathTooLongException ex)
            {
                this.Logger.WriteError("Error while ceating File Info Object for new Fax: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (NotSupportedException ex)
            {
                this.Logger.WriteError("Error while ceating File Info Object for new Fax: " + ex.ToString());
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
                        Logger.WriteInformation("Coudn´t move file. Try " + tried.ToString(CultureInfo.InvariantCulture) + " of 10!");
                        Thread.Sleep(1000);
                        fileIsMoved = false;
                    }
                    else
                    {
                        Logger.WriteError("Coundn't move file.\n" + ex.ToString());
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
                this.Logger.WriteError("Error while reading tif image: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (FileNotFoundException ex)
            {
                this.Logger.WriteError("Error while reading tif image: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (ArgumentException ex)
            {
                this.Logger.WriteError("Error while reading tif image: " + ex.ToString());
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
                    this.Logger.WriteError("Error while the ocr Prozess: " + ex.ToString());
                    this.fileSystemWatcher.EnableRaisingEvents = true;
                    return;
                }

                Operation operation = null;
                try
                {
                    // Try to parse the operation. If parsing failed, ignore this but write to the log file!
                    operation = Parser.Parse(ReplacingList, Path.Combine(_analysisPath.FullName, analyseFileName + ".txt"));

                    foreach (IJob job in Jobs)
                    {
                        try
                        {
                            // Run the job. If the job fails, ignore that exception as well but log it too!
                            if (!job.DoJob(operation))
                            {
                                this.Logger.WriteError(job.ErrorMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Be careful when processing the jobs, we don't want a malicious job to terminate the process!
                            this.Logger.WriteError(string.Format("An error occurred while processing job '{0}'. The error message was: {1}", job.GetType().Name, ex.Message));
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.WriteError("An exception occurred while parsing the alarmfax! The error message was: " + ex.Message);
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
