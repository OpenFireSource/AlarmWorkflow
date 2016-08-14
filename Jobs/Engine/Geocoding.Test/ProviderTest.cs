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

using AlarmWorkflow.Job.Geocoding.Provider;
using AlarmWorkflow.Shared.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Job.Geocoding.Test
{
    [TestClass]
    public class ProviderTest
    {
        [TestMethod]
        public void Google()
        {
            IGeoCoder g = new Google();
            GeocoderLocation location = g.Geocode(new PropertyLocation()
            {
                Street = "Marienplatz",
                StreetNumber = "8",
                City = "München",
                ZipCode = "80331"
            });
            Assert.AreEqual(48.137d, location.Latitude, 0.001);
            Assert.AreEqual(11.576d, location.Longitude, 0.001);
        }
        [TestMethod]
        public void OpenStreetMap()
        {
            IGeoCoder g = new OpenStreetMap();
            GeocoderLocation location = g.Geocode(new PropertyLocation()
            {
                Street = "Marienplatz",
                StreetNumber = "8",
                City = "München",
                ZipCode = "80331"
            });
            Assert.AreEqual(48.137d, location.Latitude, 0.001);
            Assert.AreEqual(11.576d, location.Longitude, 0.001);
        }
    }
}
