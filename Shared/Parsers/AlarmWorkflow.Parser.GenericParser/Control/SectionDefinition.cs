using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using AlarmWorkflow.Parser.GenericParser.Misc;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Control
{
    /// <summary>
    /// Represents one logical section of a fax, which contains multiple areas.
    /// </summary>
    [DebuggerDisplay("SectionString = '{SectionString}'")]
    public sealed class SectionDefinition
    {
        #region Properties

        public GenericParserString SectionString { get; set; }
        public List<SectionParserDefinition> Parsers { get; set; }
        public List<AreaDefinition> Areas { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionDefinition"/> class.
        /// </summary>
        public SectionDefinition()
        {
            SectionString = new GenericParserString();
            Parsers = new List<SectionParserDefinition>();
            Areas = new List<AreaDefinition>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionDefinition"/> class.
        /// </summary>
        /// <param name="element">The XML-element to read the data from.</param>
        public SectionDefinition(XElement element)
            : this()
        {
            this.SectionString.String = element.TryGetAttributeValue("Text", null);

            // Parse the aspects...
            foreach (XElement aspectE in element.Elements("Aspect"))
            {
                SectionParserDefinition spDefinition = new SectionParserDefinition(aspectE);
                if (string.IsNullOrWhiteSpace(spDefinition.Type))
                {
                    // TODO: Log warning
                    continue;
                }

                this.Parsers.Add(spDefinition);
            }

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

        /// <summary>
        /// Returns the AreaDefinition that that is represented by the given line.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public AreaDefinition GetArea(string line)
        {
            // TODO: Not only return just one area (introduce "AreaDefinition[] GetAreas()").
            /* 
             * It may happen that there are multiple areas
             * within one line (like "Straße: ABCDEF Haus-Nr.:")
             * then this method just returns "Straße" and merges everything
             * afterwards in the same area. 
             */
            return Areas.Find(a =>
            {
                return a.AreaString.Equals(line) ||
                    a.AreaString.StartsWith(line);
            });
        }

        /// <summary>
        /// Creates the XML-representation of this section, including all areas.
        /// </summary>
        /// <returns></returns>
        public XElement CreateXElement()
        {
            XElement se = new XElement("Section");
            se.Add(new XAttribute("Text", this.SectionString.String));

            foreach (SectionParserDefinition spd in this.Parsers)
            {
                se.Add(spd.CreateXElement());
            }

            foreach (AreaDefinition area in this.Areas)
            {
                se.Add(area.CreateXElement());
            }

            return se;
        }

        #endregion

    }
}
