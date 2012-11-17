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
            XElement ibaE = element.Element("IntroducedBy");
            this.AreaString.String = ibaE.TryGetAttributeValue("Text", null);
            this.AreaString.IsContained = ibaE.TryGetAttributeValue("IsContained", false);
            this.MapToPropertyName = ibaE.TryGetAttributeValue("MapTo", null);
            this.Separator = ibaE.TryGetAttributeValue("Separator", ":");
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

        #endregion

    }
}
