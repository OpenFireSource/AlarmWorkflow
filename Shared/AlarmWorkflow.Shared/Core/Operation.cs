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
        public string Einsatznr { get; set; }
        /// <summary>
        /// Gets or sets the Mitteiler object.
        /// </summary>
        public string Mitteiler { get; set; }
        /// <summary>
        /// Gets or sets the Einsatzort object.
        /// </summary>
        public string Einsatzort { get; set; }
        /// <summary>
        /// Gets or sets the Strasse object.
        /// </summary>
        public string Strasse { get; set; }
        /// <summary>
        /// Gets or sets the Kreuzung object.
        /// </summary>
        public string Kreuzung { get; set; }
        /// <summary>
        /// Gets or sets the Ort object.
        /// </summary>
        public string Ort { get; set; }
        /// <summary>
        /// Gets or sets the Objekt object.
        /// </summary>
        public string Objekt { get; set; }
        /// <summary>
        /// Gets or sets the Meldebild object.
        /// </summary>
        public string Meldebild { get; set; }
        /// <summary>
        /// Gets or sets the Hinweis object.
        /// </summary>
        public string Hinweis { get; set; }
        /// <summary>
        /// Gets or sets the Einsatzplan object.
        /// </summary>
        public string Einsatzplan { get; set; }
        /// <summary>
        /// Gets the Stichwort object.
        /// </summary>
        public string Stichwort { get; set; }

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
