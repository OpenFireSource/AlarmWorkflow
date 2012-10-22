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
