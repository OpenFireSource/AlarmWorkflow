using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using AlarmWorkflow.Shared.Alarmfax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Jobs;
using AlarmWorkflow.Shared.Logging;

namespace AlarmWorkflow.Shared
{
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

    /// <summary>
    /// ReplaceString struct defines a toupl of two Strings. Searching for an string an replace it with an new one.
    /// </summary>
    public struct ReplaceString
    {
        /// <summary>
        /// The new string.
        /// </summary>
        private string newString;

        /// <summary>
        /// The old string which will be replaced.
        /// </summary>
        private string oldString;

        /// <value>
        /// Gets or sets the old string.
        /// </value>
        /// <summary>
        /// Gets or sets the old string.
        /// </summary>
        public string OldString
        {
            get
            {
                return this.oldString;
            }

            set
            {
                this.oldString = value;
            }
        }

        /// <value>
        /// Gets or sets the new string.
        /// </value>
        /// <summary>
        /// Gets or sets the new string.
        /// </summary>
        public string NewString
        {
            get
            {
                return this.newString;
            }

            set
            {
                this.newString = value;
            }
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="str1">The first ReplaceString.</param>
        /// <param name="str2">The second ReplaceString.</param>
        /// <returns>Indicates if both are equal.</returns>
        public static bool operator ==(ReplaceString str1, ReplaceString str2)
        {
            return str1.Equals(str2);
        }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="str1">The first ReplaceString.</param>
        /// <param name="str2">The second ReplaceString.</param>
        /// <returns>Indicates if both are not equal.</returns>
        public static bool operator !=(ReplaceString str1, ReplaceString str2)
        {
            return !str1.Equals(str2);
        }

        /// <summary>
        /// Compares a ReplaceString struct with a object.
        /// </summary>
        /// <param name="obj">The object to compare the ReplaceString with.</param>
        /// <returns>Indicates if both are equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ReplaceString)
            {
                return this.Equals((ReplaceString)obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Compares two ReplaceString structs.
        /// </summary>
        /// <param name="str">The ReplaceString to compare with.</param>
        /// <returns>Indicates if both are equal.</returns>
        public bool Equals(ReplaceString str)
        {
            if (str.NewString == this.NewString && str.OldString == this.OldString)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Overrides the getHashCode methode. 
        /// </summary>
        /// <returns>Returns the hash code.</returns>
        public override int GetHashCode()
        {
            string str = this.OldString + this.NewString;
            return str.GetHashCode();
        }
    }

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

        /// <value>
        /// Gets or sets the logger object.
        /// </value>
        /// <summary>
        /// Gets or sets the logger object.
        /// </summary>
        internal ILogger Logger { get; set; }

        /// <value>
        /// Sets the fax path.
        /// </value>
        /// <summary>
        /// Sets the fax path.
        /// </summary>
        internal string FaxPath
        {
            set { _faxPath = new DirectoryInfo(value); }
        }

        /// <value>
        /// Sets the archiev path.
        /// </value>
        /// <summary>
        /// Sets the archiev path.
        /// </summary>
        internal string ArchivePath
        {
            set { _archivePath = new DirectoryInfo(value); }
        }

        /// <value>
        /// Sets the analysis path.
        /// </value>
        /// <summary>
        /// Sets the analysis path.
        /// </summary>
        internal string AnalysisPath
        {
            set { _analysisPath = new DirectoryInfo(value); }
        }

        /// <value>
        /// Sets the replacing list.
        /// </value>
        /// <summary>
        /// Sets the replacing list.
        /// </summary>
        internal List<ReplaceString> ReplacingList { get; set; }

        /// <value>
        /// Sets the useOCRSoftware.
        /// </value>
        /// <summary>
        /// Sets the useOCRSoftware.
        /// </summary>
        internal OcrSoftware UseOCRSoftware { get; set; }

        /// <value>
        /// Sets the useOCRSoftware.
        /// </value>
        /// <summary>
        /// Sets the useOCRSoftware.
        /// </summary>
        internal string OcrPath { get; set; }

        /// <value>
        /// Sets the parser.
        /// </value>
        /// <summary>
        /// Sets the parser.
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
        /// Makes sure that the required directories exist.
        /// </summary>
        private void EnsureDirectoriesExist()
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

        /// <summary>
        /// This Methode is started with the thread start.
        /// </summary>
        internal void DoWork()
        {
            EnsureDirectoriesExist();

            this.fileSystemWatcher = new FileSystemWatcher(_faxPath.FullName, "*.TIF");
            this.fileSystemWatcher.IncludeSubdirectories = false;
            this.fileSystemWatcher.Created += new FileSystemEventHandler(_fileSystemWatcher_Created);
            this.fileSystemWatcher.EnableRaisingEvents = true;
            this.fileSystemWatcher.WaitForChanged(WatcherChangeTypes.Created);
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

            string analyseFileName = DateTime.Now.ToString();
            analyseFileName = analyseFileName.Replace(".", string.Empty).Replace(" ", string.Empty).Replace(":", string.Empty);
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

            System.Drawing.Image img;

            try
            {
                img = System.Drawing.Image.FromFile(Path.Combine(_archivePath.FullName, analyseFileName + ".TIF"));
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

            try
            {
                img.Save(Path.Combine(_archivePath.FullName, analyseFileName + ".bmp"), System.Drawing.Imaging.ImageFormat.Bmp);
            }
            catch (ArgumentNullException ex)
            {
                this.Logger.WriteError("Error while saving tif to bmp: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }
            catch (ExternalException ex)
            {
                this.Logger.WriteError("Error while saving tif to bmp: " + ex.ToString());
                this.fileSystemWatcher.EnableRaisingEvents = true;
                return;
            }

            using (Process proc = new Process())
            {
                proc.EnableRaisingEvents = false;
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.CreateNoWindow = true;

                switch (UseOCRSoftware)
                {
                    case OcrSoftware.Tesseract:
                        {
                            proc.StartInfo.FileName = @"tesseract.exe";
                            if (String.IsNullOrEmpty(OcrPath))
                            {
                                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\tesseract";
                            }
                            else
                            {
                                proc.StartInfo.WorkingDirectory = OcrPath;
                            }

                            proc.StartInfo.Arguments = f.DirectoryName + "\\" + analyseFileName + ".bmp " + _analysisPath.FullName + analyseFileName + " -l deu";
                        }

                        break;
                    case OcrSoftware.Cuneiform:
                    default:
                        {
                            proc.StartInfo.FileName = @"cuneiform.exe";
                            if (String.IsNullOrEmpty(OcrPath))
                            {
                                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\cuneiform";
                            }
                            else
                            {
                                proc.StartInfo.WorkingDirectory = OcrPath;
                            }

                            proc.StartInfo.Arguments = @"-l ger --singlecolumn -o " + _analysisPath.FullName + analyseFileName + ".txt " + f.DirectoryName + @"\" + analyseFileName + ".bmp";
                        }

                        break;
                }

                try
                {
                    proc.Start();
                    proc.WaitForExit();
                }
                catch (ObjectDisposedException ex)
                {
                    this.Logger.WriteError("Error while the ocr Prozess: " + ex.ToString());
                    this.fileSystemWatcher.EnableRaisingEvents = true;
                    return;
                }
                catch (InvalidOperationException ex)
                {
                    this.Logger.WriteError("Error while the ocr Prozess: " + ex.ToString());
                    this.fileSystemWatcher.EnableRaisingEvents = true;
                    return;
                }
                catch (Win32Exception ex)
                {
                    this.Logger.WriteError("Error while the ocr Prozess: " + ex.ToString());
                    this.fileSystemWatcher.EnableRaisingEvents = true;
                    return;
                }

                Operation einsatz = Parser.Parse(ReplacingList, Path.Combine(_analysisPath.FullName, analyseFileName + ".txt"));
                foreach (IJob job in Jobs)
                {
                    if (!job.DoJob(einsatz))
                    {
                        this.Logger.WriteError(job.ErrorMessage);
                    }
                }
                this.fileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        #endregion
    }
}
