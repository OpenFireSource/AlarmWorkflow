using System.Collections.Generic;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Represents an operation, which was created by analyzing and parsing an incoming alarmfax.
    /// </summary>
    public class Operation
    {

        /// <value>
        /// Gets or sets the Einsatznr object.
        /// </value>
        /// <summary>
        /// Gets or sets the Einsatznr object.
        /// </summary>
        public string Einsatznr { get; set; }

        /// <value>
        /// Gets or sets the Mitteiler object.
        /// </value>
        /// <summary>
        /// Gets or sets the Mitteiler object.
        /// </summary>
        public string Mitteiler { get; set; }

        /// <value>
        /// Gets or sets the Einsatzort object.
        /// </value>
        /// <summary>
        /// Gets or sets the Einsatzort object.
        /// </summary>
        public string Einsatzort { get; set; }

        /// <value>
        /// Gets or sets the Strasse object.
        /// </value>
        /// <summary>
        /// Gets or sets the Strasse object.
        /// </summary>
        public string Strasse { get; set; }

        /// <value>
        /// Gets or sets the Kreuzung object.
        /// </value>
        /// <summary>
        /// Gets or sets the Kreuzung object.
        /// </summary>
        public string Kreuzung { get; set; }

        /// <value>
        /// Gets or sets the Ort object.
        /// </value>
        /// <summary>
        /// Gets or sets the Ort object.
        /// </summary>
        public string Ort { get; set; }

        /// <value>
        /// Gets or sets the Objekt object.
        /// </value>
        /// <summary>
        /// Gets or sets the Objekt object.
        /// </summary>
        public string Objekt { get; set; }

        /// <value>
        /// Gets or sets the Meldebild object.
        /// </value>
        /// <summary>
        /// Gets or sets the Meldebild object.
        /// </summary>
        public string Meldebild { get; set; }

        /// <value>
        /// Gets or sets the Hinweis object.
        /// </value>
        /// <summary>
        /// Gets or sets the Hinweis object.
        /// </summary>
        public string Hinweis { get; set; }

        /// <value>
        /// Gets or sets the Einsatzplan object.
        /// </value>
        /// <summary>
        /// Gets or sets the Einsatzplan object.
        /// </summary>
        public string Einsatzplan { get; set; }

        /// <value>
        /// Gets the Stichwort object.
        /// </value>
        /// <summary>
        /// Gets the Stichwort object.
        /// </summary>
        public string Stichwort { get; set; }

        /// <summary>
        /// Gets/sets additional data not covered by the basic properties in this class.
        /// </summary>
        public Dictionary<string, string> CustomData { get; set; }

    }
}
