using System;
using System.IO;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.AlarmSource.Fax.Config
{
    /// <summary>
    /// Represents the current configuration.
    /// </summary>
    internal sealed class FaxConfiguration
    {
        #region Properties

        internal string FaxPath { get; private set; }
        internal string ArchivePath { get; private set; }
        internal string AnalysisPath { get; private set; }
        internal OcrSoftware OCRSoftware { get; private set; }
        internal string OCRSoftwarePath { get; private set; }
        internal string AlarmFaxParserAlias { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FaxConfiguration"/> class.
        /// </summary>
        public FaxConfiguration()
        {
            string fileName = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Config\FaxAlarmSource.xml";
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
        }

        #endregion
    }
}
