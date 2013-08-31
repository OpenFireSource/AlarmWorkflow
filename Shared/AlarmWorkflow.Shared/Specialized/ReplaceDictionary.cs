// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Shared.Specialized
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
        /// <summary>
        /// Gets/sets whether or not the key tokens are to be interpreted as regular expressions.
        /// </summary>
        public bool InterpretAsRegex { get; set; }

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

        #region Methods

        /// <summary>
        /// Replaces all known tokens in the given input string and returns it.
        /// </summary>
        /// <param name="input">The line to replace all known tokens in. If the string is null or empty, no processing is done.</param>
        /// <returns>The string from <paramref name="input"/> with all tokens replaced.</returns>
        public string ReplaceInString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            foreach (var pair in Pairs)
            {
                input = ReplaceInStringCore(input, pair);
            }
            return input;
        }

        private string ReplaceInStringCore(string input, KeyValuePair<string, string> pair)
        {
            // Sanity-checks
            if (string.IsNullOrEmpty(pair.Key))
            {
                return input;
            }

            if (InterpretAsRegex)
            {
                // Note: If the performance is too bad, compiling regexes could be a good idea.
                Regex regex = new Regex(pair.Key);
                return regex.Replace(input, pair.Value);
            }
            return input.Replace(pair.Key, pair.Value);
        }

        #endregion

        #region IStringSettingConvertible Members

        void IStringSettingConvertible.Convert(string settingValue)
        {
            XDocument doc = XDocument.Parse(settingValue);
            InterpretAsRegex = doc.Root.TryGetAttributeValue("InterpretAsRegex", false);

            foreach (XElement rpn in doc.Root.Elements())
            {
                this.Pairs[rpn.Attribute("Old").Value] = rpn.Attribute("New").Value;
            }
        }

        string IStringSettingConvertible.ConvertBack()
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("ReplaceDictionary");
            root.Add(new XAttribute("InterpretAsRegex", InterpretAsRegex));
            doc.Add(root);

            foreach (var pair in this.Pairs)
            {
                XElement pairE = new XElement("Entry");
                pairE.Add(new XAttribute("Old", pair.Key));
                pairE.Add(new XAttribute("New", pair.Value));

                root.Add(pairE);
            }

            return doc.ToString();
        }

        #endregion
    }
}