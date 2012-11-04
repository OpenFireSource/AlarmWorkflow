using System.Collections.Generic;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.AlarmSource.Fax
{
    /// <summary>
    /// Provides a dictionary that is used to fix "misspelled" or falsely recognized strings by replacing them with other, meaningful strings, if their initial intent is known.
    /// </summary>
    public sealed class ReplaceDictionary : IStringSettingConvertible
    {
        #region Properties

        /// <summary>
        /// Gets/sets the key/value pairs (key is the source string, value is the replaced, real string used for it).
        /// </summary>
        public IDictionary<string, string> Pairs { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceDictionary"/> class.
        /// </summary>
        public ReplaceDictionary()
        {
            Pairs = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceDictionary"/> class
        /// and initializes it with the contents from an XML-file.
        /// </summary>
        /// <param name="xmlContent">Content of the XML to use for this instance.</param>
        public ReplaceDictionary(string xmlContent)
            : this()
        {
            ((IStringSettingConvertible)this).Convert(xmlContent);
        }

        #endregion

        #region IStringSettingConvertible Members

        void IStringSettingConvertible.Convert(string settingValue)
        {
            XDocument doc = XDocument.Parse(settingValue);

            foreach (XElement rpn in doc.Root.Elements())
            {
                this.Pairs[rpn.Attribute("Old").Value] = rpn.Attribute("New").Value;
            }
        }

        string IStringSettingConvertible.ConvertBack()
        {
            XDocument doc = new XDocument();
            doc.Add(new XElement("ReplaceDictionary"));

            foreach (var pair in this.Pairs)
            {
                XElement pairE = new XElement("Entry");
                pairE.Add(new XAttribute("Old", pair.Key));
                pairE.Add(new XAttribute("New", pair.Value));

                doc.Root.Add(pairE);
            }

            return doc.ToString();
        }

        #endregion
    }
}
