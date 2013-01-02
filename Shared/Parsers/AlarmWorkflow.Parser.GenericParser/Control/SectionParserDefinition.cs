using System.Collections.Generic;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Control
{
    /// <summary>
    /// Represents one section parser within a section.
    /// </summary>
    public sealed class SectionParserDefinition
    {
        #region Properties

        public string Type { get; set; }
        public IDictionary<string, string> Options { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionParserDefinition"/> class.
        /// </summary>
        public SectionParserDefinition()
        {
            Options = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionParserDefinition"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public SectionParserDefinition(XElement element)
            : this()
        {
            this.Type = element.TryGetAttributeValue("Type", null);
            foreach (var item in element.Attributes())
            {
                if (item.Name == "Type")
                {
                    continue;
                }

                Options[item.Name.LocalName] = item.Value;
            }
        }

        #endregion

        #region Methods

        internal XElement CreateXElement()
        {
            XElement element = new XElement("Aspect");
            element.Add(new XAttribute("Type", this.Type));

            foreach (var option in this.Options)
            {
                element.Add(new XAttribute(option.Key, option.Value));
            }

            return element;
        }

        #endregion
    }
}
