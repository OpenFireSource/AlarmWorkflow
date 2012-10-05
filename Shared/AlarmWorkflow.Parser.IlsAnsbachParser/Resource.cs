using System;
using System.Diagnostics;

namespace AlarmWorkflow.Parser.IlsAnsbachParser
{
    /// <summary>
    /// Represents a resource, which was was requested by the ILS.
    /// </summary>
    [Serializable()]
    [DebuggerDisplay("{Einsatzmittel}, {Alarmiert}, {GeforderteAusstattung}")]
    public sealed class Resource
    {
        /// <summary>
        /// Gets/sets the name of the resource. Usually this represents a vehicle.
        /// </summary>
        public string Einsatzmittel { get; set; }
        /// <summary>
        /// Gets/sets the timestamp of the request. May be empty.
        /// </summary>
        public string Alarmiert { get; set; }
        /// <summary>
        /// Gets/sets any equipment that is explicitely requested. May be empty.
        /// </summary>
        public string GeforderteAusstattung { get; set; }
    }
}
