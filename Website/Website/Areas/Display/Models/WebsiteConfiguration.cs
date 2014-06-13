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

using System.Web.Configuration;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared;

namespace AlarmWorkflow.Website.Reports.Areas.Display.Models
{
    /// <summary>
    /// Provides the website configuration.
    /// </summary>
    public class WebsiteConfiguration
    {
        #region Singleton

        private static WebsiteConfiguration _instance;

        /// <summary>
        /// Gets the singleton instance of this type.
        /// </summary>
        public static WebsiteConfiguration Instance
        {
            get { return _instance ?? (_instance = new WebsiteConfiguration()); }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not to display the traffic.
        /// </summary>
        public bool Traffic { get; private set; }
        /// <summary>
        /// Gets whether or not to apply tilt to the map.
        /// </summary>
        public bool Tilt { get; private set; }
        /// <summary>
        /// Gets whether or not to display the route.
        /// </summary>
        public bool Route { get; private set; }
        /// <summary>
        /// Gets the zoom control value.
        /// </summary>
        public bool ZoomControl { get; private set; }
        /// <summary>
        /// Gets the map type to use.
        /// </summary>
        public string MapType { get; private set; }
        /// <summary>
        /// Gets the map type for google.
        /// </summary>
        public string MapTypeGoogle
        {
            get { return "google.maps.MapTypeId." + MapType; }
        }
        /// <summary>
        /// Gets the home address.
        /// </summary>
        public string Home { get; private set; }
        /// <summary>
        /// Gets the maximum age of operations.
        /// </summary>
        public int MaxAge { get; private set; }
        /// <summary>
        /// Gets the update interval in milliseconds.
        /// </summary>
        public int UpdateIntervalMs { get; private set; }
        /// <summary>
        /// Gets whether or not to filter only acknowledged operations (default is true).
        /// </summary>
        public bool NonAcknowledgedOnly { get; private set; }
        /// <summary>
        /// Gets the zoom level to use for GMaps.
        /// </summary>
        public int GoogleZoomLevel { get; private set; }
        /// <summary>
        /// Gets the zoom level to use for OSM.
        /// </summary>
        public int OSMZoomLevel { get; private set; }

        #endregion

        #region Constructors

        private WebsiteConfiguration()
        {
            using (var service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(new SettingsServiceCallback()))
            {
                Home = service.Instance.GetSetting(SettingKeys.FDStreet).GetValue<string>() + " " +
                       service.Instance.GetSetting(SettingKeys.FDStreetNumber).GetValue<string>() + " " +
                       service.Instance.GetSetting(SettingKeys.FDZipCode).GetValue<string>() + " " +
                       service.Instance.GetSetting(SettingKeys.FDCity).GetValue<string>();
            }

            Traffic = WebConfigurationManager.AppSettings["Traffic"].ToLower().Equals("true");
            Tilt = WebConfigurationManager.AppSettings["Tilt"].ToLower().Equals("true");
            Route = WebConfigurationManager.AppSettings["Route"].ToLower().Equals("true");
            ZoomControl = WebConfigurationManager.AppSettings["ZoomControl"].ToLower().Equals("true");
            GoogleZoomLevel = int.Parse(WebConfigurationManager.AppSettings["GoogleZoomLevel"]);

            MapType = GetMapType();

            OSMZoomLevel = int.Parse(WebConfigurationManager.AppSettings["OSMZoomLevel"]);

            NonAcknowledgedOnly = WebConfigurationManager.AppSettings["NonAcknowledgedOnly"].ToLower().Equals("true");
            UpdateIntervalMs = int.Parse(WebConfigurationManager.AppSettings["UpdateInterval"]);
            MaxAge = int.Parse(WebConfigurationManager.AppSettings["MaxAge"]);
        }

        #endregion

        #region Methods

        private string GetMapType()
        {
            string type = WebConfigurationManager.AppSettings["MapType"].ToLower();
            switch (type)
            {
                case "straße":
                    return "ROADMAP";
                case "hybrid":
                    return "HYBRID";
                case "terrain":
                    return "TERRAIN";
                case "satellit":
                    return "SATELLITE";
            }
            return "ROADMAP";
        }

        #endregion
    }
}