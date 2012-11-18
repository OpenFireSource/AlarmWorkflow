using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Xml.Linq;
using AlarmWorkflow.Parser.GenericParser.Forms;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Misc
{
    /// <summary>
    /// Defines an area within a fax. An area is the part which controls what text is mapped to which property in an Operation.
    /// </summary>
    [DebuggerDisplay("AreaString = '{AreaString}' is mapped to property '{MapToPropertyName}'")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    sealed class AreaDefinition
    {
        #region Properties

        /// <summary>
        /// Gets/sets the string that denotes this area.
        /// </summary>
        [DisplayName("Bereichsname")]
        [Description("Der Text, der in der Zeile enthalten sein muss, damit der Bereich erkannt wird (z. B. 'Name', 'Straße' etc.).")]
        public GenericParserString AreaString { get; set; }
        /// <summary>
        /// Gets/sets the separator-string which separates the prefix with the actual value.
        /// This is usually a colon ( : ).
        /// </summary>
        [DisplayName("Trennzeichen")]
        [Description("Das Zeichen, dass den Bereich vom eigentlichen Wert trennt (üblicherweise ein Doppelpunkt).")]
        [DefaultValue(":")]
        public string Separator { get; set; }
        /// <summary>
        /// Gets/sets the name of the property in Operation where this area is mapped to.
        /// </summary>
        [DisplayName("Zugewiesene Eigenschaft")]
        [Description("Der Name der Eigenschaft, zu der dieser Bereich zugeordnet ist.")]
        [Editor(typeof(MapToPropertyUITypeEditorImpl),typeof(UITypeEditor))]
        public string MapToPropertyName { get; set; }

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
            this.AreaString.IsContained = element.TryGetAttributeValue("Text-IsContained", true);
            this.MapToPropertyName = element.TryGetAttributeValue("MapTo", null);
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
                string.IsNullOrWhiteSpace(MapToPropertyName));
        }

        /// <summary>
        /// Creates the XML-representation of this area.
        /// </summary>
        /// <returns></returns>
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
