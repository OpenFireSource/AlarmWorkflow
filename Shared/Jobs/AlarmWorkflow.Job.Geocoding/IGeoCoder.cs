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

using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Job.Geocoding
{
    /// <summary>
    ///  Exposes the interface that can be used to extend the Geocoding-Job with custom providers.
    /// </summary>
    public interface IGeoCoder
    {
        /// <summary>
        /// Pattern of the query-url. Needed for the Webrequests
        /// </summary>
        string UrlPattern { get; }
        /// <summary>
        /// Returns whether a ApiKey is required by the geocoding service or not.
        /// </summary>
        bool ApiKeyRequired { get; }
        /// <summary>
        /// ApiKey which is required by some Geocoding Services
        /// </summary>
        string ApiKey { get; set; }
        /// <summary>
        /// Runs the geocoding. Can return null!
        /// </summary>
        /// <param name="address">The address of the operation.</param>
        /// <returns>Coordinats for the given address or null if not found.</returns>
        GeocoderLocation GeoCode(PropertyLocation address);
    }
}
