using System;
using System.Runtime.Serialization;

namespace AlarmWorkflow.Windows.Service.WcfServices
{
    /// <summary>
    /// Wraps an Operation for WCF.
    /// </summary>
    [DataContract()]
    public class OperationItem
    {
        /// <summary>
        /// Gets/sets the destination city.
        /// </summary>
        [DataMember()]
        public string City { get; set; }
        /// <summary>
        /// Gets/sets the zip code of the city.
        /// </summary>
        [DataMember()]
        public string ZipCode { get; set; }
        /// <summary>
        /// Gets/sets the destination street.
        /// </summary>
        [DataMember()]
        public string Street { get; set; }
        /// <summary>
        /// Gets/sets the destination street number (optional, may be included within <see cref="P:Street"/>).
        /// </summary>
        [DataMember()]
        public string StreetNumber { get; set; }

        // TODO: Stuff from Operation!
    }
}
