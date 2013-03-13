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