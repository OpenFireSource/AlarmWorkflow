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

namespace AlarmWorkflow.Website.Reports.Areas.Display.Models
{
    /// <summary>
    /// Represents the result from resetting (acknowledging) an operation.
    /// </summary>
    public class ResetOperationData
    {
        /// <summary>
        /// Gets/sets whether or not the call was successful.
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// Gets/sets an optional message from the call.
        /// </summary>
        public string message { get; set; }
    }
}