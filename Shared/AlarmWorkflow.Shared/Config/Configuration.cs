using System;
using System.IO;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Config
{
    public sealed class Configuration
    {
        #region Fields

        private static Configuration _instance;

        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Configuration();
                }
                return _instance;
            }
        }

        #endregion

        #region Properties

        public DirectoryInfo FaxPath { get; private set; }
        public DirectoryInfo ArchivePath { get; private set; }
        public DirectoryInfo AnalysisPath { get; private set; }
        public OcrSoftware OCRSoftware { get; private set; }
        public string OCRSoftwarePath { get; private set; }
        public Type AlarmFaxParser { get; private set; }
        public Type OperationStore { get; private set; }

        #endregion

        #region Constructors

        private Configuration()
        {
            string fileName = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Config\AlarmWorkflow.xml";
            if (!File.Exists(fileName))
            {
                return;
            }

            XDocument doc = XDocument.Load(fileName);

            string faxPath = doc.Root.Element("FaxPath").Value;
            string archivePath = doc.Root.Element("ArchivePath").Value;
            string analysisPath = doc.Root.Element("AnalysisPath").Value;
            string ocr = doc.Root.Element("OCRSoftware").Attribute("type").Value;
            string ocrpath = doc.Root.Element("OCRSoftware").Attribute("path").Value;
            string parser = doc.Root.Element("AlarmfaxParser").Value;

            Configuration configuration = new Configuration();
            configuration.FaxPath = new DirectoryInfo(faxPath);
            configuration.ArchivePath = new DirectoryInfo(archivePath);
            configuration.AnalysisPath = new DirectoryInfo(analysisPath);
            configuration.OCRSoftware = (OcrSoftware)Enum.Parse(typeof(OcrSoftware), ocr);
            configuration.OCRSoftwarePath = ocrpath;

            if (string.IsNullOrWhiteSpace(parser))
            {
                configuration.OperationStore=ExportedTypeLibrary.GetExport(typeof(IParser)
            }
            else
            {

            }

            _instance = configuration;
        }

        #endregion

        #region Methods

        private IOperationStore GetOperationStore()
        {
            return ExportedTypeLibrary.Import<IOperationStore>();
        }

        #endregion
    }
}
