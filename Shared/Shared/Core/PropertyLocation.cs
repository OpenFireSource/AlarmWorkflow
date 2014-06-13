// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Text;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Defines the location of a property.
    /// </summary>
    [Serializable()]
    public sealed class PropertyLocation : IEquatable<PropertyLocation>
    {
        #region Properties

        /// <summary>
        /// Gets/sets the location name.
        /// </summary>
        public string Location { get; set; }
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
        /// Gets/sets a description of the "Intersection" (if provided by alarmsource).
        /// </summary>
        public string Intersection { get; set; }
        /// <summary>
        /// Gets/sets the latitude of the location (if provided by alarmsource).
        /// </summary>
        public string GeoLatitude { get; set; }
        /// <summary>
        /// Gets/sets the longitude of the location (if provided by alarmsource).
        /// </summary>
        public string GeoLongitude { get; set; }
        /// <summary>
        /// Gets/sets the name of the property (company, site, house etc.).
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Gets whether or not this instance represents a meaningful geographic location.
        /// This takes only ZipCode, City and Street into account. 
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
        /// Gets whether or not there are meaningful values for the geo coordinates (latitude and longitude) defined.
        /// </summary>
        public bool HasGeoCoordinates
        {
            get { return !string.IsNullOrWhiteSpace(GeoLatitude) && !string.IsNullOrWhiteSpace(GeoLongitude); }
        }

        #endregion

        #region Methods

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

        #endregion

        #region IEquatable<PropertyLocation> Members

        /// <summary>
        /// Returns whether or not this object and some other are equal.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PropertyLocation other)
        {
            return other.ToString() == this.ToString();
        }

        #endregion
    }
}