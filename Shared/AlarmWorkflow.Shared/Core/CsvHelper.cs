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
using System.Linq;
using System.Text;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Provides helper methods that deal with CSV (comma-separated value).
    /// </summary>
    public static class CsvHelper
    {
        #region Constants

        /// <summary>
        /// Defines the default char to be used within CSV.
        /// </summary>
        public const char SeparatorChar = ';';

        #endregion

        #region Methods

        /// <summary>
        /// Takes an enumerable containing custom objects, turns each into its string-representation and generates a single CSV-line from it.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>If any of the string-representations in the <paramref name="values"/>-enumerable contains the separator char
        /// as defined by <see cref="SeparatorChar"/>, then all 'columns' are wrapped within two quotiation marks to allow safe reparsing.</remarks>
        /// <param name="values">The enumerable containing the objects to use.</param>
        /// <returns>A single CSV-line generated from the <paramref name="values"/>.</returns>
        public static string ToCsvLine(IEnumerable<object> values)
        {
            if (values == null)
            {
                return null;
            }

            IList<string> strings = values.Select(val => val.ToString()).ToList();
            bool encloseInQuotationMarks = strings.Any(s => s.Contains(SeparatorChar));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strings.Count; i++)
            {
                string column = strings[i];

                if (encloseInQuotationMarks)
                {
                    sb.AppendFormat("\"{0}\"", column);
                }
                else
                {
                    sb.Append(column);
                }

                // Append separator if we are not at the last element
                if (i < strings.Count - 1)
                {
                    sb.Append(SeparatorChar);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Parses the given CSV-formatted line and returns the columns from it. Also takes quotation marks into consideration.
        /// </summary>
        /// <param name="csv">The CSV-formatted line.</param>
        /// <returns>An enumerable containing the parsed columns of the CSV line.</returns>
        public static IEnumerable<string> FromCsvLine(string csv)
        {
            if (string.IsNullOrWhiteSpace(csv))
            {
                yield break;
            }
            else
            {
                string csvtmp = csv.Trim();
                bool useQuotationMarkCheck = csvtmp.StartsWith("\"");

                string tmp = null;
                bool isInQuotationMark = false;

                for (int i = 0; i < csvtmp.Length; i++)
                {
                    char c = csvtmp[i];

                    switch (c)
                    {
                        case '"':
                            {
                                if (useQuotationMarkCheck)
                                {
                                    isInQuotationMark = !isInQuotationMark;
                                }
                                else
                                {
                                    tmp += c;
                                }
                            }
                            break;
                        default:
                            {
                                // If we encounter the separator char and we are outside a quotation mark.
                                // The main purpose that the quotation marks have is to not treat the separator char as a separator.
                                if (c == SeparatorChar && !isInQuotationMark)
                                {
                                    yield return tmp;
                                    tmp = null;
                                    break;
                                }

                                tmp += c;
                            }
                            break;
                    }
                }

                // Considers the case when there are no separators, yet there is content found.
                if (tmp != null)
                {
                    yield return tmp;
                }
            }
        }

        #endregion

    }
}