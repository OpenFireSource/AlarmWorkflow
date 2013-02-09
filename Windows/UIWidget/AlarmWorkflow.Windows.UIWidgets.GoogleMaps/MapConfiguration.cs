using System;
using AlarmWorkflow.Shared.Settings;


namespace AlarmWorkflow.Windows.UIWidgets.GoogleMaps
{
    internal class MapConfiguration
    {
        internal MapConfiguration()
        {
            Traffic = SettingsManager.Instance.GetSetting("GoogleMapsWidget", "Traffic").GetBoolean();
            Tilt = SettingsManager.Instance.GetSetting("GoogleMapsWidget", "Tilt").GetBoolean();
            Route = SettingsManager.Instance.GetSetting("GoogleMapsWidget", "Route").GetBoolean();
            ZoomControl = SettingsManager.Instance.GetSetting("GoogleMapsWidget", "ZoomControl").GetBoolean();
            ZoomLevel = SettingsManager.Instance.GetSetting("GoogleMapsWidget", "ZoomLevel").GetInt32();
            Home = SettingsManager.Instance.GetSetting("Shared", "FD.Street").GetString() + " " +
                   SettingsManager.Instance.GetSetting("Shared", "FD.StreetNumber").GetString() + " " +
                   SettingsManager.Instance.GetSetting("Shared", "FD.ZipCode").GetString() + " " +
                   SettingsManager.Instance.GetSetting("Shared", "FD.City").GetString();
            Maptype = getMapType();
        }

        internal bool Traffic { get; private set; }

        internal bool Tilt { get; private set; }

        internal bool Route { get; private set; }

        internal bool ZoomControl { get; private set; }

        internal bool RouteDescription { get; private set; }

        internal String Maptype { get; private set; }

        internal String Home { get; private set; }

        internal int ZoomLevel { get; private set; }

        private String getMapType()
        {
            String type = SettingsManager.Instance.GetSetting("GoogleMapsWidget", "MapType").GetString();
            switch (type)
            {
                case "Straﬂe":
                    return "ROADMAP";
                case "Hybrid":
                    return "HYBRID";
                case "Terrain":
                    return "TERRAIN";
                case "Satellit":
                    return "SATELLITE";
            }
            return "ROADMAP";
        }
    }
}