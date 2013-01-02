using System.Diagnostics;
using System.Xml.Linq;
using AlarmWorkflow.Parser.GenericParser.Misc;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Control
{
    /// <summary>
    /// Defines an area within a fax. An area is the part which controls what text is mapped to which property in an Operation.
    /// </summary>
    [DebuggerDisplay("AreaString = '{AreaString}' is mapped to property '{MapToPropertyExpression}'")]
    public sealed class AreaDefinition
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
        public string MapToPropertyExpression { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaDefinition"/> class.
        /// </summary>
        public AreaDefinition()
        {
            AreaString = new GenericParserString();
            Separator = ":";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaDefinition"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public AreaDefinition(XElement element)
            : this()
        {
            this.AreaString.String = element.TryGetAttributeValue("Text", null);
            this.MapToPropertyExpression = element.TryGetAttributeValue("MapTo", null);
            this.Separator = element.TryGetAttributeValue("Separator", ":");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns whether or not this area definition is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsValidDefinition()
        {
            return !(
                AreaString == null ||
                string.IsNullOrWhiteSpace(AreaString.String) ||
                string.IsNullOrWhiteSpace(MapToPropertyExpression));
        }

        /// <summary>
        /// Creates the XML-representation of this area.
        /// </summary>
        /// <returns></returns>
        public XElement CreateXElement()
        {
            XElement element = new XElement("Area");
            element.Add(new XAttribute("Text", this.AreaString.String));
            element.Add(new XAttribute("MapTo", this.MapToPropertyExpression));
            element.Add(new XAttribute("Separator", this.Separator));

            return element;
        }

        #endregion

    }
}
