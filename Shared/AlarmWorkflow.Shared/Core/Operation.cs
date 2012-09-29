using System;
using System.Collections.Generic;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Represents an operation, which was created by analyzing and parsing an incoming alarmfax.
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Defines the default timespan after which new operations/alarms are set to "acknowledged". See "IsAcknowledged"-property for further information.
        /// </summary>
        public static readonly TimeSpan DefaultAcknowledgingTimespan = TimeSpan.FromHours(8d);

        /// <summary>
        /// Gets or sets the Einsatznr object.
        /// </summary>
        public string OperationNumber { get; set; }
        /// <summary>
        /// Gets or sets the Mitteiler object.
        /// </summary>
        public string Messenger { get; set; }
        /// <summary>
        /// Gets or sets the Einsatzort object.
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Gets/sets the street. The street may contain the StreetNumber.
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// Gets/sets the street number. This may be empty and/or the street number may be merged into the Street-property.
        /// </summary>
        public string StreetNumber { get; set; }
        /// <summary>
        /// Gets/sets the Intersection, if any or applicable.
        /// </summary>
        public string Intersection { get; set; }
        /// <summary>
        /// Gets or sets the Ort object.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Gets/sets the zip code of the City.
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        /// Gets or sets the Objekt object.
        /// </summary>
        public string Property { get; set; }
        /// <summary>
        /// Gets or sets the Meldebild object.
        /// </summary>
        public string Picture { get; set; }
        /// <summary>
        /// Gets or sets the Hinweis object.
        /// </summary>
        public string Hint { get; set; }
        /// <summary>
        /// Gets or sets the Einsatzplan object.
        /// </summary>
        public string PlanOfAction { get; set; }
        /// <summary>
        /// Gets the Stichwort object.
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Gets/sets whether or not this operation is acknowledged, that means that this operation is no longer necessary to be displayed in the UI as "fresh".
        /// If this is set to "false" then this operation will always been shown in the UI. By default, an operation is set to "acknowledged"
        /// either if the user manually acknowledges it or after a defined timespan (usually 8 hours).
        /// </summary>
        public bool IsAcknowledged { get; set; }
        /// <summary>
        /// Gets/sets additional data not covered by the basic properties in this class.
        /// </summary>
        public Dictionary<string, string> CustomData { get; set; }

    }
}
