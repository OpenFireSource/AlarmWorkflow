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
using System.Web.Configuration;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Website.Asp
{
    internal class WebsiteConfiguration
    {
        #region Singleton

        private static WebsiteConfiguration _instance;

        /// <summary>
        /// Gets the singleton Instance of this type.
        /// </summary>
        public static WebsiteConfiguration Instance
        {
            get { return _instance ?? (_instance = new WebsiteConfiguration()); }
        }

        #endregion

        #region Constructors

        private WebsiteConfiguration()
        {
            try
            {
                SettingsManager.Instance.Initialize();
            }
            catch (Exception e)
            {
                //Allready initalized
            }
            Home = SettingsManager.Instance.GetSetting("Shared", "FD.Street").GetValue<string>() + " " +
                   SettingsManager.Instance.GetSetting("Shared", "FD.StreetNumber").GetValue<string>() + " " +
                   SettingsManager.Instance.GetSetting("Shared", "FD.ZipCode").GetValue<string>() + " " +
                   SettingsManager.Instance.GetSetting("Shared", "FD.City").GetValue<string>();
            //GoogleMaps
            Traffic = WebConfigurationManager.AppSettings["Traffic"].ToLower().Equals("true");
            Tilt = WebConfigurationManager.AppSettings["Tilt"].ToLower().Equals("true");
            Route = WebConfigurationManager.AppSettings["Route"].ToLower().Equals("true");
            ZoomControl = WebConfigurationManager.AppSettings["ZoomControl"].ToLower().Equals("true");
            GoogleZoomLevel = int.Parse(WebConfigurationManager.AppSettings["GoogleZoomLevel"]);
            Maptype = GetMapType();
            //OSM
            OSMZoomLevel = int.Parse(WebConfigurationManager.AppSettings["OSMZoomLevel"]);
            //Website
            NonAcknowledgedOnly = WebConfigurationManager.AppSettings["NonAcknowledgedOnly"].ToLower().Equals("true");
            UpdateIntervall = int.Parse(WebConfigurationManager.AppSettings["UpdateIntervall"]);
            MaxAge = int.Parse(WebConfigurationManager.AppSettings["MaxAge"]);
        }

        #endregion

        #region Fields

        internal bool Traffic { get; private set; }
        internal bool Tilt { get; private set; }
        internal bool Route { get; private set; }
        internal bool ZoomControl { get; private set; }
        internal bool RouteDescription { get; private set; }
        internal String Maptype { get; private set; }
        internal String Home { get; private set; }
        internal int MaxAge { get; private set; }
        internal int UpdateIntervall { get; private set; }
        internal bool NonAcknowledgedOnly { get; private set; }
        internal int GoogleZoomLevel { get; private set; }
        internal int OSMZoomLevel { get; private set; }
        #endregion

        #region Methods

        private String GetMapType()
        {
            String type = WebConfigurationManager.AppSettings["MapType"].ToLower();
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