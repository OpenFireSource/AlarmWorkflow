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

namespace AlarmWorkflow.Backend.Data.Types
{
    /// <summary>
    /// Represents a dispositioned resource.
    /// </summary>
    public class DispositionedResourceData : EntityBase
    {
        #region Properties

        /// <summary>
        /// (FK) Gets/sets the ID of the operation.
        /// </summary>
        public int OperationId { get; set; }
        /// <summary>
        /// Gets/sets the date and time of this association.
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Gets/sets the EMK resource identifier.
        /// </summary>
        public string EmkResourceId { get; set; }

        /// <summary>
        /// (Navigation property) Gets/sets the parent operation.
        /// </summary>
        public OperationData Operation { get; set; }

        #endregion
    }
}
