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

namespace AlarmWorkflow.Job.Geocoding
{
    /// <summary>
    /// Represents the result of a geocoding request.
    /// </summary>
    public class GeocoderLocation
    {
        #region Properties

        /// <summary>
        /// Gets/sets the longitude component.
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// Gets/sets the latitude component.
        /// </summary>
        public double Latitude { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the coordinates of this instance as a string in a format similar to "Latitude, Longitude".
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}", Latitude, Longitude);
        }

        #endregion
    }
}
