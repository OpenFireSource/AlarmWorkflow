using System;
using System.Collections.Generic;
using System.Text;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Provides tools to format an object's ToString()-representation.
    /// </summary>
    public static class ObjectFormatter
    {
        #region Methods

        /// <summary>
        /// Parses a string that tells how to format an object using macros within curly braces.
        /// </summary>
        /// <param name="graph">The object graph to use. Must not be null.</param>
        /// <param name="format">The format string, using the property values in curly braces, like {Property}. Must not be empty.</param>
        /// <returns>The formatted string.</returns>
        public static string ToString(object graph, string format)
        {
            Assertions.AssertNotNull(graph, "graph");
            Assertions.AssertNotEmpty(format, "format");

            StringBuilder sb = new StringBuilder(format);
            // Replace common control chars
            sb.Replace("\n", Environment.NewLine);

            foreach (string macro in GetMacros(format))
            {
                string expression = macro.Substring(1, macro.Length - 2);

                string propertyValue = "[?]";
                object rawValue = null;

                bool propertyFound = ObjectExpressionTools.TryGetValueFromExpression(graph, expression, out rawValue);
                if (propertyFound && rawValue != null)
                {
                    propertyValue = rawValue.ToString();
                }

                sb.Replace(macro, propertyValue);
            }

            return sb.ToString();
        }

        private static string[] GetMacros(string input)
        {
            List<string> list = new List<string>();

            string tmp = "";
            bool isInMacro = false;
            foreach (char c in input)
            {
                switch (c)
                {
                    case '{':
                        isInMacro = true;
                        break;
                    case '}':
                        list.Add("{" + tmp + "}");
                        tmp = "";
                        isInMacro = false;
                        break;
                    default:
                        if (isInMacro)
                        {
                            tmp += c;
                        }
                        break;
                }
            }

            return list.ToArray();
        }

        #endregion
    }
}
