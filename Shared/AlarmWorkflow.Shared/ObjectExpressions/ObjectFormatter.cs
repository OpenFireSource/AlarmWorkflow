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