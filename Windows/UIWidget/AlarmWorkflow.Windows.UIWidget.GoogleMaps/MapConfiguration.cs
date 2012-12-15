﻿using System;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Windows.UIWidget.GoogleMaps
{
    internal class MapConfiguration
    {
        internal bool Traffic { get; private set; }

        internal bool Tilt { get; private set; }

        internal bool Route { get; private set; }

        internal bool ZoomControl { get; private set; }

        internal bool RouteDescription { get; private set; }

        internal MapType Maptype { get; private set; }

        internal String Home { get; private set; }

        internal int ZoomLevel { get; private set; }

        internal MapConfiguration()
        {
            RouteDescription = SettingsManager.Instance.GetSetting("GoogleMapsWidget", "RouteDescription").GetBoolean();
            Traffic = SettingsManager.Instance.GetSetting("GoogleMapsWidget","Traffic").GetBoolean();
            Tilt = SettingsManager.Instance.GetSetting("GoogleMapsWidget","Tilt").GetBoolean();
            Route = SettingsManager.Instance.GetSetting("GoogleMapsWidget","Route").GetBoolean();
            ZoomControl = SettingsManager.Instance.GetSetting("GoogleMapsWidget","ZoomControl").GetBoolean();
            ZoomLevel = SettingsManager.Instance.GetSetting("GoogleMapsWidget", "ZoomLevel").GetInt32();
            Home = SettingsManager.Instance.GetSetting("Shared","FD.Street").GetString() + " "+
            SettingsManager.Instance.GetSetting("Shared","FD.StreetNumber").GetString() + " "+
            SettingsManager.Instance.GetSetting("Shared","FD.ZipCode").GetString() + " "+
            SettingsManager.Instance.GetSetting("Shared", "FD.City").GetString();
            Maptype = getMapType();
        }

        private MapType getMapType()
        {
            String type = SettingsManager.Instance.GetSetting("GoogleMapsWidget", "MapType").GetString();
            switch (type)
            {
                case "Straße":
                    return MapType.ROADMAP;
                case "Hybrid":
                    return MapType.HYBRID;
                case "Terrain":
                    return MapType.TERRAIN;
                case "Satellit":
                    return MapType.SATELLITE;
            }
            return MapType.ROADMAP;
        }
    }
}