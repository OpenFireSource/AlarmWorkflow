using System.Collections.Generic;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Provides helper methods that deal with option strings, that is, a string that consists of options in "option=value;option=value..." format.
    /// </summary>
    public static class OptionStringHelper
    {
        #region Constants

        /// <summary>
        /// Defines the character that separates the pairs from each other.
        /// </summary>
        public const char PairSeparatorChar = ';';
        /// <summary>
        /// Defines the character that separates the key from the value within a pair.
        /// </summary>
        public const char PairInnerDelimiterChar = '=';

        #endregion

        #region Methods

        /// <summary>
        /// Parses the option string and returns all options and values as a dictionary.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>The option string shall be in a format similar to <c>key=value;key=value</c>.
        /// If an inner pair is in invalid form, it will be skipped.</remarks>
        /// <param name="optionString">The option string.</param>
        /// <returns>A dictionary containing the keys and pairs from the <paramref name="optionString"/>.
        /// Returns an empty dictionary if the <paramref name="optionString"/> was either null or empty.</returns>
        public static IDictionary<string, string> GetAsPairs(string optionString)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(optionString))
            {
                string[] pairs = optionString.Split(PairSeparatorChar);
                for (int i = 0; i < pairs.Length; i++)
                {                    
                    string[] tokens = pairs[i].Split(PairInnerDelimiterChar);
                    if (tokens.Length == 2)
                    {
                        result[tokens[0]] = tokens[1];
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
