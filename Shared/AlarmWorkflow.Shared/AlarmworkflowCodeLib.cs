using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Xml;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Jobs;
using AlarmWorkflow.Shared.Logging;
using AlarmWorkflow.Shared.Alarmfax;

namespace AlarmWorkflow.Shared
{
    /// <summary>
    /// Description of AlarmworkflowCodeLib.
    /// </summary>
    public class AlarmworkflowClass
    {
        #region Fields

        private ILogger _logger;
        private Thread _workingThread;
        private WorkingThread _workingThreadInstance;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the AlarmworkflowClass class.
        /// Constructor is reading the XML File, and safe the Settings in to the WorkingThread Instance.
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public AlarmworkflowClass()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Content\AlarmWorkflow.xml");

            // Initialising Logger
            XmlNode node = doc.GetElementsByTagName("Logging")[0];
            string loggerstr = node.SelectSingleNode("Logger").Attributes["type"].Value;
            switch (loggerstr.ToUpperInvariant())
            {
                case "EVENTLOG":
                    {
                        this.Logger = new EventLogLogger();
                        this.Logger.IsEnabled = true;
                    }

                    break;
                case "NONE":
                default:
                    {
                        this.Logger = new NoLogger();
                        this.Logger.IsEnabled = false;
                    }

                    break;
            }

            if (!this.Logger.Initialize())
            {
                throw new InvalidProgramException("Exepection occurred during Logger initiialization!");
            }

            this.Logger.WriteInformation("Starting Service");

            // Thread Einstellungen initiieren
            node = doc.GetElementsByTagName("Service")[0];
            string faxPath = node.SelectSingleNode("FaxPath").InnerText;
            string archievPath = node.SelectSingleNode("ArchievPath").InnerText;
            string analysisPath = node.SelectSingleNode("AnalysisPath").InnerText;
            string ocr = node.SelectSingleNode("OCRSoftware").Attributes["type"].InnerText;
            string ocrpath = node.SelectSingleNode("OCRSoftware").Attributes["path"].InnerText;
            string parser = node.SelectSingleNode("AlarmfaxParser").InnerText;

            this.WorkingThreadInstance = new WorkingThread();

            this.WorkingThreadInstance.FaxPath = faxPath;
            this.WorkingThreadInstance.ArchievPath = archievPath;
            this.WorkingThreadInstance.Logger = this.Logger;
            this.WorkingThreadInstance.AnalysisPath = analysisPath;
            if (ocr.ToUpperInvariant() == "TESSERACT")
            {
                this.WorkingThreadInstance.UseOCRSoftware = OcrSoftware.Tesseract;
            }
            else
            {
                this.WorkingThreadInstance.UseOCRSoftware = OcrSoftware.Cuneiform;
            }

            this.WorkingThreadInstance.OcrPath = ocrpath;

            // AktiveJobs Options
            XmlNodeList aktiveJobsList = doc.GetElementsByTagName("AktiveJobs");
            XmlNode aktiveJobs = aktiveJobsList[0];
            string debugAktiveString = aktiveJobs.Attributes["DebugMode"].InnerText;
            bool debugaktive = false;
            if (debugAktiveString.ToUpperInvariant() == "TRUE")
            {
                debugaktive = true;
            }

            bool databaseAktive = true;
            bool smsAktive = true;
            bool mailAktive = true;
            bool replaceAktive = true;
            bool displayWakeUpAktive = true;
            foreach (XmlNode xnode in aktiveJobs.ChildNodes)
            {
                switch (xnode.Name)
                {
                    case "Database":
                        if (xnode.Attributes["aktive"].InnerText == "true")
                        {
                            databaseAktive = true;
                        }
                        else
                        {
                            databaseAktive = false;
                        }

                        break;
                    case "SMS":
                        if (xnode.Attributes["aktive"].InnerText == "true")
                        {
                            smsAktive = true;
                        }
                        else
                        {
                            smsAktive = false;
                        }

                        break;
                    case "Mailing":
                        if (xnode.Attributes["aktive"].InnerText == "true")
                        {
                            mailAktive = true;
                        }
                        else
                        {
                            mailAktive = false;
                        }

                        break;
                    case "Replacing":
                        if (xnode.Attributes["aktive"].InnerText == "true")
                        {
                            replaceAktive = true;
                        }
                        else
                        {
                            replaceAktive = false;
                        }

                        break;
                    case "DisplayWakeUp":
                        if (xnode.Attributes["aktive"].InnerText == "true")
                        {
                            displayWakeUpAktive = true;
                        }
                        else
                        {
                            displayWakeUpAktive = false;
                        }

                        break;
                    default:
                        break;
                }
            }

            InitializeJobs(doc, debugaktive, databaseAktive, smsAktive, mailAktive, displayWakeUpAktive);

            List<ReplaceString> rplist = new List<ReplaceString>();
            if (replaceAktive)
            {
                XmlNode replacingNode = doc.GetElementsByTagName("replacing")[0];
                foreach (XmlNode rpn in replacingNode.ChildNodes)
                {
                    ReplaceString rps = new ReplaceString();
                    rps.OldString = rpn.Attributes["old"].InnerText;
                    rps.NewString = rpn.Attributes["new"].InnerText;
                    rplist.Add(rps);
                }

                this.WorkingThreadInstance.ReplacingList = rplist;
            }

            // Import parser with the given name/alias
            _workingThreadInstance.Parser = ExportedTypeLibrary.Import<IParser>(parser);
        }

        #endregion

        private void InitializeJobs(XmlDocument doc, bool debugaktive, bool databaseAktive, bool smsAktive, bool mailAktive, bool displayWakeUpAktive)
        {
            // NOTE: TENTATIVE CODE until settings are stored more dynamical!
            XmlNode jobsSettings = doc.GetElementsByTagName("Jobs")[0];

            // TODO: Jobs to be defined in settings dynamically! --> remove the hardcodings...
            if (databaseAktive)
            {
                // TODO: Hardcoded - to be defined in the job's settings!
                this.WorkingThreadInstance.Jobs.Add(ExportedTypeLibrary.Import<IJob>("DatabaseJob"));
            }

            if (displayWakeUpAktive)
            {
                //this.WorkingThreadInstance.Jobs.Add(ExportedTypeLibrary.Import<IJob>("DisplayWakeUp"));
            }

            if (mailAktive)
            {
                // TODO: Hardcoded - to be defined in the job's settings!
                this.WorkingThreadInstance.Jobs.Add(ExportedTypeLibrary.Import<IJob>("MailingJob"));
            }

            if (smsAktive)
            {
                //this.WorkingThreadInstance.Jobs.Add(ExportedTypeLibrary.Import<IJob>("SmsJob"));
            }

            // Initialize all jobs
            foreach (IJob job in _workingThreadInstance.Jobs)
            {
                job.Initialize(jobsSettings);
            }
        }

        /// <summary>
        /// Gets or sets the workingThread object.
        /// </summary>
        /// <value>
        /// Gets or sets the workingThread object.
        /// </value>
        public Thread WorkerThread
        {
            get { return this._workingThread; }
            set { this._workingThread = value; }
        }

        /// <summary>
        /// Gets or sets the workingThreadInstance object.
        /// </summary>
        internal WorkingThread WorkingThreadInstance
        {
            get { return this._workingThreadInstance; }
            set { this._workingThreadInstance = value; }
        }

        /// <summary>
        /// Gets or sets the logger object.
        /// </summary>
        internal ILogger Logger
        {
            get { return this._logger; }
            set { this._logger = value; }
        }

        /// <summary>
        /// Starts the monitor thread, which is waiting for a new Alarm.
        /// </summary>
        public void Start()
        {
            this.WorkerThread = new Thread(new ThreadStart(this.WorkingThreadInstance.DoWork));
            this.WorkerThread.Start();
            this.Logger.WriteInformation("Started Service");
        }

        /// <summary>
        /// Stops the Thread.
        /// </summary>
        public void Stop()
        {
            this.Logger.WriteInformation("Stopping Service");
            if (this.WorkerThread != null)
            {
                this.WorkerThread.Abort();
            }

            this.Logger.WriteInformation("Stopped Service");
        }
    }
}
