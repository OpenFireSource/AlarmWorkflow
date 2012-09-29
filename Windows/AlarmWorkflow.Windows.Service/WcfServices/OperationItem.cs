using System.Collections.Generic;
using System.Runtime.Serialization;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.Service.WcfServices
{
    /// <summary>
    /// Wraps an Operation for WCF.
    /// </summary>
    [DataContract()]
    public class OperationItem
    {

        #region Properties

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
        /// Gets or sets the Einsatzort object.
        /// </summary>
        [DataMember()]
        public string Location { get; set; }
        /// <summary>
        /// Gets/sets the street. The street may contain the StreetNumber.
        /// </summary>
        [DataMember()]
        public string Street { get; set; }
        /// <summary>
        /// Gets/sets the street number. This may be empty and/or the street number may be merged into the Street-property.
        /// </summary>
        [DataMember()]
        public string StreetNumber { get; set; }
        /// <summary>
        /// Gets/sets the Intersection, if any or applicable.
        /// </summary>
        [DataMember()]
        public string Intersection { get; set; }
        /// <summary>
        /// Gets or sets the Ort object.
        /// </summary>
        [DataMember()]
        public string City { get; set; }
        /// <summary>
        /// Gets/sets the zip code of the City.
        /// </summary>
        [DataMember()]
        public string ZipCode { get; set; }
        /// <summary>
        /// Gets or sets the Objekt object.
        /// </summary>
        [DataMember()]
        public string Property { get; set; }
        /// <summary>
        /// Gets or sets the Meldebild object.
        /// </summary>
        [DataMember()]
        public string Picture { get; set; }
        /// <summary>
        /// Gets or sets the Hinweis object.
        /// </summary>
        [DataMember()]
        public string Hint { get; set; }
        /// <summary>
        /// Gets or sets the Einsatzplan object.
        /// </summary>
        [DataMember()]
        public string PlanOfAction { get; set; }
        /// <summary>
        /// Gets the Stichwort object.
        /// </summary>
        [DataMember()]
        public string Keyword { get; set; }
        /// <summary>
        /// Gets/sets whether or not this operation is acknowledged, that means that this operation is no longer necessary to be displayed in the UI as "fresh".
        /// If this is set to "false" then this operation will always been shown in the UI. By default, an operation is set to "acknowledged"
        /// either if the user manually acknowledges it or after a defined timespan (usually 8 hours).
        /// </summary>
        [DataMember()]
        public bool IsAcknowledged { get; set; }
        /// <summary>
        /// Gets/sets additional data not covered by the basic properties in this class.
        /// </summary>
        [DataMember()]
        public Dictionary<string, string> CustomData { get; set; }

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
            this.OperationNumber = operation.OperationNumber;
            this.Messenger = operation.Messenger;
            this.Location = operation.Location;
            this.Street = operation.Street;
            this.StreetNumber = operation.StreetNumber;
            this.Intersection = operation.Intersection;
            this.City = operation.City;
            this.ZipCode = operation.ZipCode;
            this.Property = operation.Property;
            this.Picture = operation.Picture;
            this.Hint = operation.Hint;
            this.PlanOfAction = operation.PlanOfAction;
            this.Keyword = operation.Keyword;
            this.IsAcknowledged = operation.IsAcknowledged;
            this.CustomData = operation.CustomData;
        }

        #endregion

    }
}
