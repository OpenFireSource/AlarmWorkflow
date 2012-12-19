#region

using System;
using System.Collections.Generic;
using System.Windows.Documents;

#endregion

namespace AlarmWorkflow.Windows.UIWidgets.Operation
{
    internal class Helper
    {
        internal static Inline Traverse(string value)
        {
            string[] sections = SplitIntoSections(value);
            if (sections.Length.Equals(1))
            {
                string section = sections[0];
                string token;
                int tokenStart, tokenEnd;
                if (GetTokenInfo(section, out token, out tokenStart, out tokenEnd))
                {
                    string content = token.Length.Equals(tokenEnd - tokenStart) ?
                                         null :
                                         section.Substring(token.Length, section.Length - 1 - token.Length*2);

                    switch (token.ToLower())
                    {
                        case "<bold>":
                            return new Bold(Traverse(content));
                        case "<italic>":
                            return new Italic(Traverse(content));
                        case "<underline>":
                            return new Underline(Traverse(content));
                        case "<linebreak/>":
                            return new LineBreak();
                        default:
                            return new Run(section);
                    }
                }
                return new Run(section);
            }
            Span span = new Span();

            foreach (string section in sections)
                span.Inlines.Add(Traverse(section));

            return span;
        }

        /// <summary>
        ///     Examines the passed string and find the first token, where it begins and where it ends.
        /// </summary>
        /// <param name="value">The string to examine.</param>
        /// <param name="token">The found token.</param>
        /// <param name="startIndex">Where the token begins.</param>
        /// <param name="endIndex">Where the end-token ends.</param>
        /// <returns>True if a token was found.</returns>
        private static bool GetTokenInfo(string value, out string token, out int startIndex, out int endIndex)
        {
            token = null;
            endIndex = -1;

            startIndex = value.IndexOf("<");
            int startTokenEndIndex = value.IndexOf(">");
            if (startIndex < 0)
                return false;
            if (startTokenEndIndex < 0)
                return false;
            token = value.Substring(startIndex, startTokenEndIndex - startIndex + 1);
            if (token.EndsWith("/>"))
            {
                endIndex = startIndex + token.Length;
                return true;
            }

            string endToken = token.Insert(1, "/");
            int nesting = 0;
            int pos = 0;
            do
            {
                int temp_startTokenIndex = value.IndexOf(token, pos, StringComparison.Ordinal);
                int temp_endTokenIndex = value.IndexOf(endToken, pos, StringComparison.Ordinal);

                if (temp_startTokenIndex >= 0 && temp_startTokenIndex < temp_endTokenIndex)
                {
                    nesting++;
                    pos = temp_startTokenIndex + token.Length;
                }
                else if (temp_endTokenIndex >= 0 && nesting > 0)
                {
                    nesting--;
                    pos = temp_endTokenIndex + endToken.Length;
                }
                else // Invalid tokenized string
                    return false;
            } while (nesting > 0);

            endIndex = pos;

            return true;
        }

        /// <summary>
        ///     Splits the string into sections of tokens and regular text.
        /// </summary>
        /// <param name="value">The string to split.</param>
        /// <returns>An array with the sections.</returns>
        private static string[] SplitIntoSections(string value)
        {
            List<string> sections = new List<string>();

            while (!string.IsNullOrEmpty(value))
            {
                string token;
                int tokenStartIndex, tokenEndIndex;
                if (GetTokenInfo(value, out token, out tokenStartIndex, out tokenEndIndex))
                {
                    if (tokenStartIndex > 0)
                        sections.Add(value.Substring(0, tokenStartIndex));

                    sections.Add(value.Substring(tokenStartIndex, tokenEndIndex - tokenStartIndex));
                    value = value.Substring(tokenEndIndex);
                }
                else
                {
                    sections.Add(value);
                    value = null;
                }
            }

            return sections.ToArray();
        }
    }
}