
namespace AlarmWorkflow.Shared.ObjectExpressions
{
    /// <summary>
    /// Provides tools to format an object's ToString()-representation.
    /// See documentation for further information.
    /// </summary>
    /// <remarks>This class provides a basic wrapper around the more versatile class <see cref="ObjectExpressionFormatter&lt;TInput&gt;"/></remarks>
    public static class ObjectFormatter
    {
        #region Methods

        /// <summary>
        /// Parses a string that tells how to format an object using macros within curly braces.
        /// </summary>
        /// <typeparam name="T">The type of the object to format.</typeparam>
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
        /// <typeparam name="T">The type of the object to format.</typeparam>
        /// <param name="graph">The object graph to use. Must not be null.</param>
        /// <param name="format">The format string, using the property values in curly braces, like {Property}. Must not be empty.</param>
        /// <param name="options">Controls the formatting process.</param>
        /// <returns>The formatted string.</returns>
        public static string ToString<T>(T graph, string format, ObjectFormatterOptions options)
        {
            return ObjectExpressionFormatter<T>.ToString(graph, format, options);
        }

        #endregion
    }
}
