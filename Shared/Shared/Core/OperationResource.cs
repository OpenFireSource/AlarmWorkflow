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
using System.Collections.Generic;
using System.Diagnostics;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Represents a resource, which was was requested by the call center.
    /// </summary>
    [Serializable()]
    [DebuggerDisplay("Name = {FullName}, at {Timestamp} (Amount = {RequestedEquipment.Count})")]
    public sealed class OperationResource
    {
        #region Properties

        /// <summary>
        /// Gets/sets the name of the resource. Usually this represents a vehicle.
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Gets/sets the timestamp of the request. May be empty.
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// Gets/sets any equipment that is explicitely requested. May be empty.
        /// </summary>
        public List<string> RequestedEquipment { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResource"/> class.
        /// </summary>
        public OperationResource()
        {
            RequestedEquipment = new List<string>();
        }

        #endregion
    }
}