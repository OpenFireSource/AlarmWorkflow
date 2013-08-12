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