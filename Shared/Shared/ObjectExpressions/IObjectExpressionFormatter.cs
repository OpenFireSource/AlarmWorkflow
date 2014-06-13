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
    /// Exposes the basic formatting mechanism of the object expression formatting types.
    /// </summary>
    public interface IObjectExpressionFormatter
    {
        /// <summary>
        /// Gets/sets the options that shall be used for formatting.
        /// </summary>
        ObjectFormatterOptions Options { get; set; }
        /// <summary>
        /// Parses a string that tells how to format an object using macros within curly braces.
        /// </summary>
        /// <param name="graph">The object graph to use. Must not be null.</param>
        /// <param name="format">The format string, using the property values in curly braces (expressions), like {Property}. Must not be empty.</param>
        /// <returns>The formatted string.</returns>
        string ToString(object graph, string format);
    }
}
