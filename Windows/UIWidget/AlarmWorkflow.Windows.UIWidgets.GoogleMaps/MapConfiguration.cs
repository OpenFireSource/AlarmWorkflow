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
                case "Straße":
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