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

using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Extensibility
{
    /// <summary>
    /// Defines a parser which parses a string[].
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Parses the contents of the given lines into an <see cref="Operation"/>-representation.
        /// </summary>
        /// <param name="lines">The line contents of the analysed file.</param>
        /// <returns>The operation-instance that contains the data from the fax.</returns>
        Operation Parse(string[] lines);
    }
}