using System;
using System.IO;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Config
{
    /// <summary>
    /// Represents the current configuration.
    /// </summary>
    public sealed class Configuration
    {
        #region Fields

        private static Configuration _instance;
        /// <summary>
        /// Gets the current Configuration.
        /// </summary>
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

        internal string FaxPath { get; private set; }
        internal string ArchivePath { get; private set; }
        internal string AnalysisPath { get; private set; }
        internal OcrSoftware OCRSoftware { get; private set; }
        internal string OCRSoftwarePath { get; private set; }
        internal string AlarmFaxParserAlias { get; private set; }
        internal string OperationStoreAlias { get; private set; }
        internal bool DownloadRoutePlan { get; private set; }
        internal int RouteImageHeight { get; private set; }
        internal int RouteImageWidth { get; private set; }

        public FireDepartmentInfo FDInformation { get; private set; }

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

            string ocr = doc.Root.Element("OCRSoftware").Attribute("type").Value;
            this.OCRSoftware = (OcrSoftware)Enum.Parse(typeof(OcrSoftware), ocr);
            this.OCRSoftwarePath = doc.Root.Element("OCRSoftware").Attribute("path").Value;

            this.FaxPath = doc.Root.Element("FaxPath").Value;
            this.ArchivePath = doc.Root.Element("ArchivePath").Value;
            this.AnalysisPath = doc.Root.Element("AnalysisPath").Value;
            this.AlarmFaxParserAlias = doc.Root.Element("AlarmfaxParser").Value;
            this.OperationStoreAlias = doc.Root.Element("OperationStore").Value;

            this.DownloadRoutePlan = bool.Parse(doc.Root.Element("DownloadRoutePlan").Value);
            this.RouteImageHeight = int.Parse(doc.Root.Element("RouteImageHeight").Value);
            this.RouteImageWidth = int.Parse(doc.Root.Element("RouteImageWidth").Value);

            // FD Information
            {
                XElement fdInfoE = doc.Root.Element("FDInformation");
                this.FDInformation = new FireDepartmentInfo();
                this.FDInformation.Name = fdInfoE.Element("Name").Value;
                this.FDInformation.Location = new PropertyLocation();
                this.FDInformation.Location.ZipCode = fdInfoE.Element("ZipCode").Value;
                this.FDInformation.Location.City = fdInfoE.Element("City").Value;
                this.FDInformation.Location.Street = fdInfoE.Element("Street").Value;
                this.FDInformation.Location.StreetNumber = fdInfoE.Element("StreetNumber").Value;
            }

            _instance = this;
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents information about the current fire department site.
        /// </summary>
        public class FireDepartmentInfo
        {
            /// <summary>
            /// Gets the name of the site.
            /// </summary>
            public string Name { get; internal set; }
            /// <summary>
            /// Gets the location of the site.
            /// </summary>
            public PropertyLocation Location { get; internal set; }
        }


        #endregion
    }
}
