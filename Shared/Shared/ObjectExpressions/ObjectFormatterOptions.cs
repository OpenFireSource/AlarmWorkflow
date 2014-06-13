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

namespace AlarmWorkflow.Shared.ObjectExpressions
{
    /// <summary>
    /// Specifies the options that can be used to alter the way the <see cref="T:ObjectFormatter" /> formats an object.
    /// </summary>
    [Flags()]
    public enum ObjectFormatterOptions
    {
        /// <summary>
        /// Uses no options. This is the default value.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Removes newline characters from the format string, resulting in one long line.
        /// </summary>
        RemoveNewlines = 1,
        /// <summary>
        /// Inserts a question mark '[?]' for any value that is <c>null</c>.
        /// If this option is not specified, nothing is inserted (empty).
        /// </summary>
        InsertQuestionMarksForNullValues = 2,
    }
}