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

using AlarmWorkflow.BackendService.ManagementContracts.Emk;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Website.Reports.Areas.Display.Models
{
    /// <summary>
    /// A object for displaying resources in the alarmpage
    /// </summary>
    public class ResourceObject
    {
        #region Properties

        /// <summary>
        /// A <see cref="EmkResource"/> if any is assigned to the resource
        /// </summary>
        public EmkResource Emk { get; set; }

        /// <summary>
        /// The <see cref="OperationResource"/> which should be displayed
        /// </summary>
        public OperationResource Resource { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor for creating a new <see cref="ResourceObject"/>
        /// </summary>
        /// <param name="emk">The filtered entry. Can be null!</param>
        /// <param name="resource">The original entry. Can't be null!</param>
        public ResourceObject(EmkResource emk, OperationResource resource)
        {
            Assertions.AssertNotNull(resource, "resource");

            Emk = emk;
            Resource = resource;
        }

        #endregion

    }
}
