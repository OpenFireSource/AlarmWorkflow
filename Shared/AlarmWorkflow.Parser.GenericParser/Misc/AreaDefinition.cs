using System.Diagnostics;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Misc
{
    /// <summary>
    /// Defines an area within a fax. An area is the part which controls what text is mapped to which property in an Operation.
    /// </summary>
    [DebuggerDisplay("AreaString = '{AreaString}' is mapped to property '{MapToPropertyName}'")]
    class AreaDefinition
    {
        #region Properties

        /// <summary>
        /// Gets/sets the string that denotes this area.
        /// </summary>
        public GenericParserString AreaString { get; set; }
        /// <summary>
        /// Gets/sets the separator-string which separates the prefix with the actual value.
        /// This is usually a colon ( : ).
        /// </summary>
        public string Separator { get; set; }
        /// <summary>
        /// Gets/sets the name of the property in Operation where this area is mapped to.
        /// </summary>
        public string MapToPropertyName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaDefinition"/> class.
        /// </summary>
        public AreaDefinition()
        {
            AreaString = new GenericParserString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaDefinition"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public AreaDefinition(XElement element)
            : this()
        {
            this.AreaString.String = element.TryGetAttributeValue("Text", null);
            this.AreaString.IsContained = element.TryGetAttributeValue("Text-IsContained", true);
            this.MapToPropertyName = element.TryGetAttributeValue("MapTo", null);
            this.Separator = element.TryGetAttributeValue("Separator", ":");
        }

        #endregion

        #region Methods

        public bool IsValidDefinition()
        {
            return !(
                AreaString == null ||
                string.IsNullOrWhiteSpace(AreaString.String) ||
                string.IsNullOrWhiteSpace(MapToPropertyName));
        }

        public XElement CreateXElement()
        {
            XElement element = new XElement("Area");
            element.Add(new XAttribute("Text", this.AreaString.String));
            element.Add(new XAttribute("Text-IsContained", this.AreaString.IsContained));
            element.Add(new XAttribute("MapTo", this.MapToPropertyName));
            element.Add(new XAttribute("Separator", this.Separator));

            return element;
        }

        #endregion

    }
}
