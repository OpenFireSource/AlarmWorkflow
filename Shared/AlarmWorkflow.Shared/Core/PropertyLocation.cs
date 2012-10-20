using System;
using System.Text;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Defines the location of a property.
    /// </summary>
    [Serializable()]
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

        /// <summary>
        /// Returns a string describing the property location like: "[[Street] [StreetNumber]], [ZipCode] [City]".
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(Street))
            {
                sb.Append(Street);

                if (!string.IsNullOrWhiteSpace(StreetNumber))
                {
                    sb.Append(" ");
                    sb.Append(StreetNumber);
                }
                sb.Append(", ");
            }

            bool hasZipCode = !string.IsNullOrWhiteSpace(ZipCode);
            if (hasZipCode)
            {
                sb.Append(ZipCode);
            }

            if (!string.IsNullOrWhiteSpace(City))
            {
                if (hasZipCode)
                {
                    sb.Append(" ");
                }

                sb.Append(City);
            }

            return sb.ToString().Trim();
        }
    }
}
