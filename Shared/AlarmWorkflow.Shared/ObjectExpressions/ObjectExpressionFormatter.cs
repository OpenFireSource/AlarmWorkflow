using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Properties;

namespace AlarmWorkflow.Shared.ObjectExpressions
{
    /// <summary>
    /// Provides a means to format any object using a user-defined string that consists of expressions.
    /// See documentation for further information.
    /// </summary>
    /// <remarks>A user-defined string may be built like follows: <c>Id: {Id}, Comment: {Comment}, Location: {Einsatzort}</c>.
    /// There is a possibility to define a custom resolving method, which gets called in case of one expression leading to no
    /// resolvable property.</remarks>
    /// <typeparam name="TInput">The type of the object to format.</typeparam>
    public sealed class ObjectExpressionFormatter<TInput>
    {
        #region Constants

        /// <summary>
        /// Represents the string that is inserted if a value is null. Only used if the corresponding option is specified.
        /// </summary>
        private const string NullValueString = "[?]";

        #endregion

        #region Fields

        private ExpressionResolver<TInput> _resolver;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the options that shall be used for formatting.
        /// </summary>
        public ObjectFormatterOptions Options { get; set; }
        /// <summary>
        /// Gets/sets the <see cref="IFormatProvider"/> to use for formatting values.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>By default, the format provider from <see cref="System.Globalization.CultureInfo.CurrentCulture"/> is used.</remarks>
        public IFormatProvider FormatProvider { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectExpressionFormatter&lt;TInput&gt;"/> class.
        /// </summary>
        public ObjectExpressionFormatter()
        {
            Options = ObjectFormatterOptions.Default;
            FormatProvider = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectExpressionFormatter&lt;TInput&gt;"/> class.
        /// </summary>
        /// <param name="resolver">The resolver that shall be used when an expression could not be resolved.</param>
        public ObjectExpressionFormatter(ExpressionResolver<TInput> resolver)
            : this()
        {
            _resolver = resolver;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses a string that tells how to format an object using macros within curly braces.
        /// </summary>
        /// <param name="graph">The object graph to use. Must not be null.</param>
        /// <param name="format">The format string, using the property values in curly braces (expressions), like {Property}. Must not be empty.</param>
        /// <returns>The formatted string.</returns>
        public string ToString(TInput graph, string format)
        {
            Assertions.AssertNotNull(graph, "graph");
            Assertions.AssertNotEmpty(format, "format");

            string nullValueString = "";
            if (Options.HasFlag(ObjectFormatterOptions.InsertQuestionMarksForNullValues))
            {
                nullValueString = NullValueString;
            }

            StringBuilder sb = new StringBuilder(format);

            if (Options.HasFlag(ObjectFormatterOptions.RemoveNewlines))
            {
                sb.Replace("\n", " ");
                sb.Replace(Environment.NewLine, " ");
            }

            foreach (string macro in GetMacros(format))
            {
                string expression = macro.Substring(1, macro.Length - 2);

                string propertyValue = nullValueString;
                object rawValue = null;

                bool propertyFound = ObjectExpressionTools.TryGetValueFromExpression(graph, expression, out rawValue);
                if (propertyFound)
                {
                    if (rawValue != null)
                    {
                        propertyValue = rawValue.ToString();
                    }
                }
                else
                {
                    ResolveExpressionResult result = TryCallResolver(graph, expression);
                    if (result.Success && result.ResolvedValueHasValue)
                    {
                        propertyValue = result.ResolvedValue.ToString();
                    }
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

        private ResolveExpressionResult TryCallResolver(TInput graph, string expression)
        {
            if (_resolver != null)
            {
                try
                {
                    return _resolver(graph, expression);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Resources.ErrorWhileInvokingUserExpressionResolver);
                    Logger.Instance.LogException(this, ex);
                }
            }

            return ResolveExpressionResult.Fail;
        }

        /// <summary>
        /// Parses a string that tells how to format an object using macros within curly braces.
        /// </summary>
        /// <param name="graph">The object graph to use. Must not be null.</param>
        /// <param name="format">The format string, using the property values in curly braces, like {Property}. Must not be empty.</param>
        /// <returns>The formatted string.</returns>
        public static string ToString<T>(T graph, string format)
        {
            return ToString(graph, format, ObjectFormatterOptions.Default);
        }

        /// <summary>
        /// Parses a string that tells how to format an object using macros within curly braces.
        /// </summary>
        /// <param name="graph">The object graph to use. Must not be null.</param>
        /// <param name="format">The format string, using the property values in curly braces, like {Property}. Must not be empty.</param>
        /// <param name="options">Controls the formatting process.</param>
        /// <returns>The formatted string.</returns>
        public static string ToString<T>(T graph, string format, ObjectFormatterOptions options)
        {
            ObjectExpressionFormatter<T> formatter = new ObjectExpressionFormatter<T>();
            formatter.Options = options;
            return formatter.ToString(graph, format);
        }

        #endregion
    }
}
