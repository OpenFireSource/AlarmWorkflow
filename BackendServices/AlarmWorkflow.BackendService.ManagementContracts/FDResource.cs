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

using System.Diagnostics;
using System.Runtime.Serialization;

namespace AlarmWorkflow.BackendService.ManagementContracts
{
    /// <summary>
    /// Specifies the details of a single unit (vehicle/resource/etc.) of a fire department.
    /// </summary>
    [DataContract()]
    [DebuggerDisplay("Identifier = {Identifier}, DisplayName = {DisplayName}")]
    public sealed class FDResource
    {
        #region Properties

        /// <summary>
        /// Gets/sets the unique identifier. This usually matches the name
        /// from the alarm source for this resource and is contained within <see cref="P:OperationResource.FullName"/>.
        /// </summary>
        [DataMember()]
        public string Identifier { get; set; }
        /// <summary>
        /// Gets/sets the display friendly name for this unit.
        /// </summary>
        [DataMember()]
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets/sets the path of the image representing this resource, to be used in UI components.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>This path shall be relative to the ProgramData-directory and not be absolute.</remarks>
        [DataMember()]
        public string ImagePath { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FDResource"/> struct.
        /// </summary>
        public FDResource()
        {
            Identifier = string.Empty;
            DisplayName = string.Empty;
        }

        #endregion
    }
}
