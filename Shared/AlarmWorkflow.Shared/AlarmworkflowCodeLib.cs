using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Xml;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Shared
{
    /// <summary>
    /// Represents the main entry point of the AlarmWorkflow application. This class manages the parsing of the Alarmfaxes and is responsible for calling the configured jobs.
    /// </summary>
    public sealed class AlarmworkflowClass : IDisposable
    {
        #region Fields

        private ExtensionManager _extensionManager;

        private Thread _workingThread;
        private WorkingThread _workingThreadInstance;
        private IOperationStore _operationStore;

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not the AlarmWorkflow-process is running.
        /// </summary>
        public bool IsStarted { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the AlarmworkflowClass class.
        /// Constructor is reading the XML File, and safe the Settings in to the WorkingThread Instance.
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public AlarmworkflowClass()
        {
            _extensionManager = new ExtensionManager();

            XmlDocument doc = new XmlDocument();
            doc.Load(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Config\AlarmWorkflow.xml");

            // Thread Einstellungen initiieren
            XmlNode node = doc.GetElementsByTagName("Service")[0];
            string faxPath = node.SelectSingleNode("FaxPath").InnerText;
            string archievPath = node.SelectSingleNode("ArchievPath").InnerText;
            string analysisPath = node.SelectSingleNode("AnalysisPath").InnerText;
            string ocr = node.SelectSingleNode("OCRSoftware").Attributes["type"].InnerText;
            string ocrpath = node.SelectSingleNode("OCRSoftware").Attributes["path"].InnerText;
            string parser = node.SelectSingleNode("AlarmfaxParser").InnerText;

            _workingThreadInstance = new WorkingThread();

            _workingThreadInstance.FaxPath = faxPath;
            _workingThreadInstance.ArchivePath = archievPath;
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

            InitializeJobs(doc);

            // Import parser with the given name/alias
            _workingThreadInstance.Parser = _extensionManager.GetExtensionWithName<IParser>(parser);
            Logger.Instance.LogFormat(LogType.Info, this, "Using parser '{0}'.", _workingThreadInstance.Parser.GetType().FullName);
        }

        #endregion

        #region Methods

        private void InitializeJobs(XmlDocument doc)
        {
            // NOTE: TENTATIVE CODE until settings are stored more dynamical!
            XmlNode jobsSettings = doc.GetElementsByTagName("Jobs")[0];

            foreach (IJob job in _extensionManager.GetExtensionsOfType<IJob>())
            {
                string jobName = job.GetType().Name;
                Logger.Instance.LogFormat(LogType.Info, this, "Initializing job type '{0}'...", jobName);

                try
                {
                    job.Initialize(jobsSettings);
                    _workingThreadInstance.Jobs.Add(job);

                    Logger.Instance.LogFormat(LogType.Info, this, "Job type '{0}' initialization successful.", jobName);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, "An error occurred while initializing job type '{0}'. The error message was: {1}", jobName, ex.Message);
                }
            }
        }

        /// <summary>
        /// Returns the instance of <see cref="IOperationStore"/> that was registered to be used.
        /// </summary>
        /// <returns></returns>
        public IOperationStore GetOperationStore()
        {
            // TODO: Load from setings!
            if (_operationStore == null)
            {
                _operationStore = Utilities.FirstOrDefault(_extensionManager.GetExtensionsOfType<IOperationStore>());
            }
            return _operationStore;
        }

        /// <summary>
        /// Starts the monitor thread, which is waiting for a new Alarm.
        /// </summary>
        public void Start()
        {
            if (IsStarted)
            {
                return;
            }

            _workingThread = new Thread(new ThreadStart(_workingThreadInstance.DoWork));
            _workingThread.Start();
            Logger.Instance.LogFormat(LogType.Info, this, "Started Service");

            IsStarted = true;
        }

        /// <summary>
        /// Stops the Thread.
        /// </summary>
        public void Stop()
        {
            if (!IsStarted)
            {
                return;
            }

            Logger.Instance.LogFormat(LogType.Info, this, "Stopping Service");
            if (_workingThread != null)
            {
                _workingThread.Abort();
            }

            Logger.Instance.LogFormat(LogType.Info, this, "Stopped Service");

            IsStarted = false;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();

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
                if (_workingThreadInstance != null)
                {
                    _workingThreadInstance.Dispose();
                    _workingThreadInstance = null;
                }
            }
        }

        #endregion
    }
}
