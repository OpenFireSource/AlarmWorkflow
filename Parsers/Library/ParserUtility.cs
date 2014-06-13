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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AlarmWorkflow.Parser.Library
{
    /// <summary>
    /// Provides some utility methods for the usage within the parser library.
    /// </summary>
    public static class ParserUtility
    {
        #region Constants

        private const string StreetNumberRegex = @"\d+[ ]*[a-z]*";
        private const string DateRegex = @"(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d";
        private const string TimeRegex = @"([01]?[0-9]|2[0-3]):[0-5][0-9]";
        private static readonly string[] DateTimeParsingTokens = new string[] { "dd.MM.yyyy HH1mm", "dd.MM.yyyy HH:mm" };
        private static readonly string[] StreetTokens = new string[] { @"A\d+", @"B\d+", @"St\d+" };

        #endregion

        #region Methods

        /// <summary>
        /// Attempts to parse the timestamp from a line, using a well-known date and time format.
        /// </summary>
        /// <param name="line">The line of text to parse.</param>
        /// <param name="fallback">The <see cref="DateTime"/>-instance to use as the fallback, if parsing was unsuccessful.</param>
        /// <returns>A <see cref="DateTime"/>-instance that contains a usable timestamp.</returns>
        public static DateTime ReadFaxTimestamp(string line, DateTime fallback)
        {
            DateTime date = fallback;
            TimeSpan timestamp = date.TimeOfDay;

            Match dt = Regex.Match(line, DateRegex);
            Match ts = Regex.Match(line, TimeRegex);
            if (dt.Success)
            {
                DateTime.TryParse(dt.Value, out date);
            }
            if (ts.Success)
            {
                TimeSpan.TryParse(ts.Value, out timestamp);
            }

            return new DateTime(date.Year, date.Month, date.Day, timestamp.Hours, timestamp.Minutes, timestamp.Seconds, timestamp.Milliseconds, DateTimeKind.Local);
        }

        /// <summary>
        /// Determines whether or not the given line starts with a keyword from the given keyword-collection.
        /// </summary>
        /// <param name="line">The line to check for keywords.</param>
        /// <param name="keywords">The list of keywords to use.</param>
        /// <param name="keyword">If the line starts with a keyword from the keywords-list, this parameter contains the keyword. Otherwise, null.</param>
        /// <returns>A boolean value indicating whether or not the given line starts with a keyword from the keywords-collection.</returns>
        public static bool StartsWithKeyword(string line, IEnumerable<string> keywords, out string keyword)
        {
            line = line.ToUpperInvariant();
            foreach (string kwd in keywords)
            {
                if (line.StartsWith(kwd, StringComparison.CurrentCultureIgnoreCase))
                {
                    keyword = kwd;
                    return true;
                }
            }
            keyword = null;
            return false;
        }

        /// <summary>
        /// Returns the message text, which is the line text but excluding the keyword/prefix and a possible colon.
        /// </summary>
        /// <param name="line">The line text to retrieve the message text from.</param>
        /// <param name="prefix">The prefix that is to be removed (optional).</param>
        /// <returns></returns>
        public static string GetMessageText(string line, string prefix)
        {
            if (prefix == null)
            {
                prefix = "";
            }

            if (prefix.Length > 0)
            {
                line = line.Remove(0, prefix.Length).Trim();
            }
            else
            {
                int colonIndex = line.IndexOf(':');
                if (colonIndex != -1)
                {
                    line = line.Remove(0, colonIndex + 1);
                }
            }

            if (line.StartsWith(":"))
            {
                line = line.Remove(0, 1).Trim();
            }

            return line;
        }

        /// <summary>
        /// Attempts to read the zip code from the city, if available.
        /// </summary>
        /// <param name="cityText">The city; may or may not contain the zip code prefixing the name.</param>
        /// <returns>The zip code of the city. -or- null, if there was no.</returns>
        public static string ReadZipCodeFromCity(string cityText)
        {
            string zipCode = "";
            foreach (char c in cityText)
            {
                if (char.IsNumber(c))
                {
                    zipCode += c;
                    continue;
                }
                break;
            }
            return zipCode;
        }

        /// <summary>
        /// Attempts to convert the given message into a datetime.
        /// </summary>
        /// <param name="message">The message that presumably represents a timestamp.</param>
        /// <param name="fallback">The <see cref="DateTime"/>-instance to use if parsing failed.</param>
        /// <returns>A valid <see cref="DateTime"/>, either parsed from the message or representing the fallback value.</returns>
        public static DateTime TryGetTimestampFromMessage(string message, DateTime fallback)
        {
            DateTime dt = fallback;
            foreach (string token in DateTimeParsingTokens)
            {
                if (DateTime.TryParseExact(token, message, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return dt;
                }
            }

            return fallback;
        }

        /// <summary>
        /// Removes an existing trailing newline from the given string.
        /// </summary>
        /// <param name="value">The string to remove an existing trailing newline from.</param>
        /// <returns></returns>
        public static string RemoveTrailingNewline(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.EndsWith("\n"))
            {
                return value.Substring(0, value.Length - 1).Trim();
            }

            return value;
        }

        /// <summary>
        /// Splits the provided 'streetline' into the parts street, streetnumber and appendix. 
        /// It also checks whether we're maybe ordered on a highway or not. In that case the kilometer is stored in the field streetnumber.
        /// </summary>
        /// <param name="line">The line from the alarmfax which should be splited.</param>
        /// <param name="street">The street found in the line.</param>
        /// <param name="streetNumber">Either a house number or the kilometer on the highway.</param>
        /// <param name="appendix">The 'rest' behind the house number e.g. the floor or further information about the location.</param>
        public static void AnalyzeStreetLine(string line, out string street, out string streetNumber, out string appendix)
        {
            int index;
            Match item;
            int length;
            streetNumber = "1";
            appendix = string.Empty;
            Match match = Regex.Match(line, "Haus-?Nr.:");
            if (IsHighway(line))
            {
                if (!match.Success)
                {
                    MatchCollection matchCollections = Regex.Matches(line, " \\d+");
                    if (matchCollections.Count <= 0)
                    {
                        streetNumber = string.Empty;
                    }
                    else
                    {
                        item = matchCollections[matchCollections.Count - 1];
                        streetNumber = item.Value.Trim();
                        line = line.Remove(item.Index, item.Value.Length);
                    }
                }
                else
                {
                    index = match.Index;
                    streetNumber = line.Substring(index + match.Value.Length).Trim();
                    line = line.Substring(0, index).Trim();
                }
                street = line.Trim();
            }
            else if (!match.Success)
            {
                item = Regex.Match(line, StreetNumberRegex);
                if (!item.Success)
                {
                    street = line;
                }
                else
                {
                    streetNumber = item.Value.Trim();
                    street = line.Substring(0, item.Index).Trim();
                    length = item.Value.Length + item.Index;
                    appendix = line.Substring(length).Trim();
                }
            }
            else
            {
                index = match.Index;
                street = line.Substring(0, index).Trim();
                line = line.Substring(index).Trim();
                item = Regex.Match(line, StreetNumberRegex);
                if (item.Success)
                {
                    streetNumber = item.Value.Trim();
                    length = item.Value.Length + item.Index;
                    appendix = line.Substring(length).Trim();
                }
            }
        }

        private static bool IsHighway(string line)
        {
            return StreetTokens.Any(x => Regex.IsMatch(line, x));
        }

        /// <summary>
        /// Gets the text between two strings. Provides the possibilty to use StringComparison.
        /// </summary>
        /// <param name="line">The text which should be used for searching</param>
        /// <param name="from">The start string</param>
        /// <param name="until">The end string</param>
        /// <param name="comparison">The type of comparing/finding the needles.</param>
        /// <returns>The inner string.</returns>
        public static string GetTextBetween(string line, string from = null, string until = null, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            int length = (from ?? string.Empty).Length;
            int num = (!string.IsNullOrEmpty(from) ? line.IndexOf(from, comparison) + length : 0);
            if (num < length)
            {
                throw new ArgumentException("From: Failed to find an instance of the first anchor");
            }
            int num1 = (!string.IsNullOrEmpty(until) ? line.IndexOf(until, num, comparison) : line.Length);
            if (num1 < 0)
            {
                throw new ArgumentException("Until: Failed to find an instance of the last anchor");
            }
            string str = line.Substring(num, num1 - num);
            return str.Trim();
        }

        #endregion
    }
}
