using System.Collections.Generic;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Control
{
    /// <summary>
    /// Represents the control information used to analyze a fax.
    /// </summary>
    public sealed class ControlInformation
    {
        #region Properties

        public string FaxName { get; set; }
        public List<SectionDefinition> Sections { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlInformation"/> class.
        /// </summary>
        public ControlInformation()
        {
            Sections = new List<SectionDefinition>();
        }

        #endregion

        #region Methods

        public SectionDefinition GetSection(string sectionName)
        {
            return Sections.Find(s => s.SectionString.Equals(sectionName));
        }

        /// <summary>
        /// Saves the current instance to the given file.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("ControlInformation");
            doc.Add(root);

            root.Add(new XAttribute("Name", this.FaxName));
            foreach (SectionDefinition section in this.Sections)
            {
                root.Add(section.CreateXElement());
            }


            doc.Save(fileName);
        }

        /// <summary>
        /// Loads the control information from the given file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ControlInformation Load(string fileName)
        {
            ControlInformation ci = new ControlInformation();

            XDocument doc = XDocument.Load(fileName);
            ci.FaxName = doc.Root.TryGetAttributeValue("Name", null);

            // Parse the sections...
            foreach (XElement sectionE in doc.Root.Elements("Section"))
            {
                ci.Sections.Add(new SectionDefinition(sectionE));
            }

            return ci;
        }

        #endregion
    }
}
