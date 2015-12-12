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
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Backend.Data.Types
{
    /// <summary>
    /// Represents an operation.
    /// </summary>
    public class OperationData : EntityBase
    {
        #region Properties

        /// <summary>
        /// Gets/sets whether or not this operation has been acknowledged.
        /// </summary>
        public bool IsAcknowledged { get; set; }
        /// <summary>
        /// Gets/sets a <see cref="Guid"/> uniquely identifying this operation.
        /// </summary>
        public Guid Guid { get; set; }
        /// <summary>
        /// Gets/sets an arbitrary operation number (filled by the alarm source).
        /// </summary>
        public string OperationNumber { get; set; }
        /// <summary>
        /// Gets/sets the date and time this operation was received by the system.
        /// </summary>
        public DateTime IncomeAt { get; set; }
        /// <summary>
        /// Gets/sets the date and time the underlying alarm was initially raised.
        /// </summary>
        public DateTime AlarmAt { get; set; }
        /// <summary>
        /// Gets/sets the operation messenger.
        /// </summary>
        public string Messenger { get; set; }
        /// <summary>
        /// Gets/sets an arbitrary comment.
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Gets/sets the operation plan.
        /// </summary>
        public string Plan { get; set; }
        /// <summary>
        /// Gets/sets the operation picture.
        /// </summary>
        public string Picture { get; set; }
        /// <summary>
        /// Gets/sets the operation priority.
        /// </summary>
        public string Priority { get; set; }
        /// <summary>
        /// Gets/sets the "Einsatzort" (place of action).  Usually this location contains the destination spot.
        /// </summary>
        public PropertyLocation Einsatzort { get; set; }
        /// <summary>
        /// Gets/sets the "Zielort" (destination location).  This is usually empty.
        /// </summary>
        public PropertyLocation Zielort { get; set; }
        /// <summary>
        /// Gets/sets the keywords associated with this operation.
        /// </summary>
        public OperationKeywords Keywords { get; set; }
        /// <summary>
        /// Gets/sets the loops that are associated with this operation.
        /// </summary>
        public string Loops { get; set; }
        /// <summary>
        /// Gets/sets custom data (filled by the alarm source).
        /// </summary>
        public string CustomData { get; set; }

        /// <summary>
        /// (Navigation property)
        /// </summary>
        public virtual ICollection<OperationResourceData> Resources { get; set; }
        /// <summary>
        /// (Navigation property)
        /// </summary>
        public virtual ICollection<DispositionedResourceData> DispositionedResources { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationData"/> class.
        /// </summary>
        public OperationData()
        {
            Resources = new HashSet<OperationResourceData>();
            DispositionedResources = new HashSet<DispositionedResourceData>();

            Einsatzort = new PropertyLocation();
            Zielort = new PropertyLocation();
            Keywords = new OperationKeywords();
        }

        #endregion
    }
}
