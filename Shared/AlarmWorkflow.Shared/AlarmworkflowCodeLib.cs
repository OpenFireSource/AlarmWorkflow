using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Xml;
using AlarmWorkflow.Shared.Alarmfax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Jobs;
using AlarmWorkflow.Shared.Logging;

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
                        _logger = new EventLogLogger();
                        _logger.IsEnabled = true;
                    }

                    break;
                case "NONE":
                default:
                    {
                        _logger = new NoLogger();
                        _logger.IsEnabled = false;
                    }

                    break;
            }

            if (!_logger.Initialize())
            {
                throw new InvalidProgramException("Exepection occurred during Logger initiialization!");
            }

            _logger.WriteInformation("Starting Service");

            // Thread Einstellungen initiieren
            node = doc.GetElementsByTagName("Service")[0];
            string faxPath = node.SelectSingleNode("FaxPath").InnerText;
            string archievPath = node.SelectSingleNode("ArchievPath").InnerText;
            string analysisPath = node.SelectSingleNode("AnalysisPath").InnerText;
            string ocr = node.SelectSingleNode("OCRSoftware").Attributes["type"].InnerText;
            string ocrpath = node.SelectSingleNode("OCRSoftware").Attributes["path"].InnerText;
            string parser = node.SelectSingleNode("AlarmfaxParser").InnerText;

            _workingThreadInstance = new WorkingThread();

            _workingThreadInstance.FaxPath = faxPath;
            _workingThreadInstance.ArchievPath = archievPath;
            _workingThreadInstance.Logger = _logger;
            _workingThreadInstance.AnalysisPath = analysisPath;
            if (ocr.ToUpperInvariant() == "TESSERACT")
            {
                _workingThreadInstance.UseOCRSoftware = OcrSoftware.Tesseract;
            }
            else
            {
                _workingThreadInstance.UseOCRSoftware = OcrSoftware.Cuneiform;
            }

            _workingThreadInstance.OcrPath = ocrpath;

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

                _workingThreadInstance.ReplacingList = rplist;
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
                _workingThreadInstance.Jobs.Add(ExportedTypeLibrary.Import<IJob>("DatabaseJob"));
            }

            if (displayWakeUpAktive)
            {
                //_workingThreadInstance.Jobs.Add(ExportedTypeLibrary.Import<IJob>("DisplayWakeUp"));
            }

            if (mailAktive)
            {
                // TODO: Hardcoded - to be defined in the job's settings!
                _workingThreadInstance.Jobs.Add(ExportedTypeLibrary.Import<IJob>("MailingJob"));
            }

            if (smsAktive)
            {
                //_workingThreadInstance.Jobs.Add(ExportedTypeLibrary.Import<IJob>("SmsJob"));
            }

            // Initialize all jobs
            foreach (IJob job in _workingThreadInstance.Jobs)
            {
                job.Initialize(jobsSettings);
            }
        }

        /// <summary>
        /// Starts the monitor thread, which is waiting for a new Alarm.
        /// </summary>
        public void Start()
        {
            _workingThread = new Thread(new ThreadStart(_workingThreadInstance.DoWork));
            _workingThread.Start();
            _logger.WriteInformation("Started Service");
        }

        /// <summary>
        /// Stops the Thread.
        /// </summary>
        public void Stop()
        {
            _logger.WriteInformation("Stopping Service");
            if (_workingThread != null)
            {
                _workingThread.Abort();
            }

            _logger.WriteInformation("Stopped Service");
        }
    }
}
