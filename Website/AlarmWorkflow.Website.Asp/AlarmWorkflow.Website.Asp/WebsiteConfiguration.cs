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
        ///     Gets the singleton Instance of this type.
        /// </summary>
        public static WebsiteConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WebsiteConfiguration();
                }
                return _instance;
            }
        }

        #endregion

        private WebsiteConfiguration()
        {
            SettingsManager.Instance.Initialize();
            Home = SettingsManager.Instance.GetSetting("Shared", "FD.Street").GetString() + " " +
                   SettingsManager.Instance.GetSetting("Shared", "FD.StreetNumber").GetString() + " " +
                   SettingsManager.Instance.GetSetting("Shared", "FD.ZipCode").GetString() + " " +
                   SettingsManager.Instance.GetSetting("Shared", "FD.City").GetString();
            //GoogleMaps
            Traffic = WebConfigurationManager.AppSettings["Traffic"].ToLower().Equals("true");
            Tilt = WebConfigurationManager.AppSettings["Tilt"].ToLower().Equals("true");
            Route = WebConfigurationManager.AppSettings["Route"].ToLower().Equals("true");
            ZoomControl = WebConfigurationManager.AppSettings["ZoomControl"].ToLower().Equals("true");
            GoogleZoomLevel = int.Parse(WebConfigurationManager.AppSettings["GoogleZoomLevel"]);
            Maptype = getMapType();
            //Website
            NonAcknowledgedOnly = WebConfigurationManager.AppSettings["NonAcknowledgedOnly"].ToLower().Equals("true");
            UpdateIntervall = int.Parse(WebConfigurationManager.AppSettings["UpdateIntervall"]);
            MaxAge = int.Parse(WebConfigurationManager.AppSettings["MaxAge"]);
        }

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

        #region Methods

        private String getMapType()
        {
            String type = WebConfigurationManager.AppSettings["MapType"].ToLower();
            switch (type)
            {
                case "straﬂe":
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