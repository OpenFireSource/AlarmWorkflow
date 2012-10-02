using System;

namespace AlarmWorkflow.Windows.UI.Models
{
    /// <summary>
    /// Defines the location of a property.
    /// </summary>
    public class PropertyLocation
    {
        /// <summary>
        /// Gets/sets the zip code of the city.
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        /// Gets/sets the city name.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Gets/sets the street. May contain the street number.
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// Gets/sets the street number. May be contained within the street.
        /// </summary>
        public string StreetNumber { get; set; }

        /// <summary>
        /// Gets whether or not this instance represents a meaningful location.
        /// </summary>
        public bool IsMeaningful
        {
            get
            {
                return ((!string.IsNullOrWhiteSpace(ZipCode) || !string.IsNullOrWhiteSpace(City))
                    && !string.IsNullOrWhiteSpace(Street));
            }
        }
    }
}
