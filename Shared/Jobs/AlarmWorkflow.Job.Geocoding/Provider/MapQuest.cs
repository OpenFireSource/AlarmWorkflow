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

using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Job.Geocoding.Provider
{
    [Export("MapQuest", typeof(IGeoCoder))]
    [Information(DisplayName = "ExportMapQuestDisplayName", Description = "ExportMapQuestDescription")]
    class MapQuest : IGeoCoder
    {
        #region IGeoCoder Members

        string IGeoCoder.UrlPattern
        {
            get { return "http://www.mapquestapi.com/geocoding/v1/address?outFormat=xml&key={0}&location={1}"; }
        }

        bool IGeoCoder.IsApiKeyRequired
        {
            get { return true; }
        }

        string IGeoCoder.ApiKey { get; set; }

        GeocoderLocation IGeoCoder.Geocode(PropertyLocation address)
        {
            string queryAdress = string.Format(((IGeoCoder)this).UrlPattern, ((IGeoCoder)this).ApiKey, HttpUtility.UrlEncode(address.ToString()));

            WebRequest request = WebRequest.Create(queryAdress);
            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    XDocument document = XDocument.Load(stream);

                    XElement longitudeElement = document.Descendants("lng").FirstOrDefault();
                    XElement latitudeElement = document.Descendants("lat").FirstOrDefault();

                    if (longitudeElement != null && latitudeElement != null)
                    {
                        return new GeocoderLocation()
                        {
                            Longitude = double.Parse(longitudeElement.Value, CultureInfo.InvariantCulture),
                            Latitude = double.Parse(latitudeElement.Value, CultureInfo.InvariantCulture)
                        };
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
