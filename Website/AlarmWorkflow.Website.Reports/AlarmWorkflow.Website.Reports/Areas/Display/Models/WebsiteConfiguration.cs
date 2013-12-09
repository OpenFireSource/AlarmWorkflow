using System;
using System.Web.Configuration;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Website.Reports.Areas.Display.Models
{
    /// <summary>
    /// Represents the configuration for the website.
    /// </summary>
    public class WebsiteConfiguration
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
            Home = SettingsManager.Instance.GetSetting("Shared", "FD.Street").GetString() + " " +
                   SettingsManager.Instance.GetSetting("Shared", "FD.StreetNumber").GetString() + " " +
                   SettingsManager.Instance.GetSetting("Shared", "FD.ZipCode").GetString() + " " +
                   SettingsManager.Instance.GetSetting("Shared", "FD.City").GetString();

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