using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.ServiceContracts
{
    /// <summary>
    /// Wraps an Operation for WCF.
    /// </summary>
    [DataContract()]
    public class OperationItem
    {
        #region Fields

        [DataMember()]
        private byte[] _customData;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the unique Id of this operation.
        /// </summary>
        [DataMember()]
        public int Id { get; set; }
        /// <summary>
        /// Gets/sets the operation guid.
        /// </summary>
        [DataMember()]
        public Guid OperationGuid { get; set; }
        /// <summary>
        /// Gets/sets the date and time of the actual alarm.
        /// </summary>
        [DataMember()]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Gets/sets the date and time when this operation was created.
        /// </summary>
        [DataMember()]
        public DateTime TimestampIncome { get; set; }
        /// <summary>
        /// Gets or sets the Einsatznr object.
        /// </summary>
        [DataMember()]
        public string OperationNumber { get; set; }
        /// <summary>
        /// Gets or sets the Mitteiler object.
        /// </summary>
        [DataMember()]
        public string Messenger { get; set; }
        /// <summary>
        /// Gets or sets the Hinweis object.
        /// </summary>
        [DataMember()]
        public string Comment { get; set; }
        /// <summary>
        /// Gets/sets the priority of this operation.
        /// </summary>
        [DataMember()]
        public string Priority { get; set; }
        /// <summary>
        /// Gets/sets the "Einsatzort" (place of action).
        /// Usually this location contains the destination spot.
        /// </summary>
        [DataMember()]
        public PropertyLocation Einsatzort { get; set; }
        /// <summary>
        /// Gets/sets the "Zielort" (destination location).
        /// This is usually empty.
        /// </summary>
        [DataMember()]
        public PropertyLocation Zielort { get; set; }
        /// <summary>
        /// Gets/sets the keywords.
        /// </summary>
        [DataMember()]
        public OperationKeywords Keywords { get; set; }
        /// <summary>
        /// Gets/sets the picture.
        /// </summary>
        [DataMember()]
        public string Picture { get; set; }
        /// <summary>
        /// Gets/sets the operation plan.
        /// </summary>
        [DataMember()]
        public string OperationPlan { get; set; }
        /// <summary>
        /// Gets/sets whether or not this operation is acknowledged, that means that this operation is no longer necessary to be displayed in the UI as "fresh".
        /// If this is set to "false" then this operation will always been shown in the UI. By default, an operation is set to "acknowledged"
        /// either if the user manually acknowledges it or after a defined timespan (usually 8 hours).
        /// </summary>
        [DataMember()]
        public bool IsAcknowledged { get; set; }
        /// <summary>
        /// Gets/sets the information about all resources (vehicles, requested equipment etc.).
        /// </summary>
        [DataMember()]
        public OperationResourceCollection Resources { get; set; }
        /// <summary>
        /// Gets/sets the custom data.
        /// </summary>
        public IDictionary<string, object> CustomData
        {
            get { return Utilities.Deserialize<IDictionary<string, object>>(_customData); }
            set { _customData = Utilities.Serialize(value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationItem"/> class.
        /// </summary>
        public OperationItem()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationItem"/> class.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/> to clone.</param>
        public OperationItem(Operation operation)
            : this()
        {
            this.Id = operation.Id;
            this.OperationGuid = operation.OperationGuid;
            this.Timestamp = operation.Timestamp;
            this.TimestampIncome = operation.TimestampIncome;
            this.OperationNumber = operation.OperationNumber;
            this.Messenger = operation.Messenger;
            this.Comment = operation.Comment;
            this.Einsatzort = operation.Einsatzort;
            this.Zielort = operation.Zielort;
            this.Priority = operation.Priority;
            this.Keywords = operation.Keywords;
            this.Picture = operation.Picture;
            this.OperationPlan = operation.OperationPlan;
            this.Resources = operation.Resources;
            this.IsAcknowledged = operation.IsAcknowledged;
            this.CustomData = operation.CustomData;
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// Converts this instance to its corresponding <see cref="Operation"/>-object.
        /// </summary>
        /// <returns></returns>
        public Operation ToOperation()
        {
            Operation operation = new Operation();
            operation.Id = this.Id;
            operation.OperationGuid = this.OperationGuid;
            operation.Timestamp = this.Timestamp;
            operation.TimestampIncome = this.TimestampIncome;
            operation.OperationNumber = this.OperationNumber;
            operation.Messenger = this.Messenger;
            operation.Einsatzort = this.Einsatzort;
            operation.Zielort = this.Zielort;
            operation.Keywords = this.Keywords;
            operation.Priority = this.Priority;
            operation.Comment = this.Comment;
            operation.OperationPlan = this.OperationPlan;
            operation.Picture = this.Picture;
            operation.Resources = this.Resources;
            operation.CustomData = this.CustomData;
            operation.IsAcknowledged = this.IsAcknowledged;
            return operation;
        }

        #endregion
    }
}
