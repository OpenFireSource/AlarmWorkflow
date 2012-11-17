using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Misc
{
    [DebuggerDisplay("SectionString = '{SectionString}'")]
    class SectionDefinition
    {
        #region Properties

        public GenericParserString SectionString { get; set; }
        public List<AreaDefinition> Areas { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionDefinition"/> class.
        /// </summary>
        public SectionDefinition()
        {
            SectionString = new GenericParserString();
            Areas = new List<AreaDefinition>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionDefinition"/> class.
        /// </summary>
        /// <param name="element">The XML-element to read the data from.</param>
        public SectionDefinition(XElement element)
            : this()
        {
            XElement ibsE = element.Element("IntroducedBy");

            this.SectionString.String = ibsE.Value;
            this.SectionString.IsContained = ibsE.TryGetAttributeValue("IsContained", false);

            // Parse the areas...
            foreach (XElement areaE in element.Elements("Area"))
            {
                AreaDefinition areaDefinition = new AreaDefinition(areaE);
                if (!areaDefinition.IsValidDefinition())
                {
                    // TODO: Log warning
                    continue;
                }

                this.Areas.Add(areaDefinition);
            }
        }

        #endregion

        #region Methods

        public AreaDefinition GetArea(string line)
        {
            return Areas.Find(a =>
            {
                return a.AreaString.Equals(line) ||
                    a.AreaString.StartsWith(line);
            });
        }

        #endregion

    }
}
