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
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Settings;
using SharedSettingKeys = AlarmWorkflow.Shared.SettingKeys;

namespace AlarmWorkflow.Windows.UIWidgets.GoogleMaps
{
    class MapConfiguration
    {
        internal MapConfiguration()
        {
            using (var service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(new SettingsServiceCallback()))
            {
                GoogleMapsKey = service.Instance.GetSetting(SettingKeys.GoogleMapsKey).GetValue<string>();
                Traffic = service.Instance.GetSetting(SettingKeys.Traffic).GetValue<bool>();
                Tilt = service.Instance.GetSetting(SettingKeys.Tilt).GetValue<bool>();
                Route = service.Instance.GetSetting(SettingKeys.Route).GetValue<bool>();
                ZoomControl = service.Instance.GetSetting(SettingKeys.ZoomControl).GetValue<bool>();
                ZoomLevel = service.Instance.GetSetting(SettingKeys.ZoomLevel).GetValue<int>();
                ZoomOnAddress = service.Instance.GetSetting(SettingKeys.ZoomOnAddress).GetValue<bool>();
                Home = service.Instance.GetSetting(SharedSettingKeys.FDStreet).GetValue<string>() + " " +
                       service.Instance.GetSetting(SharedSettingKeys.FDStreetNumber).GetValue<string>() + " " +
                       service.Instance.GetSetting(SharedSettingKeys.FDZipCode).GetValue<string>() + " " +
                       service.Instance.GetSetting(SharedSettingKeys.FDCity).GetValue<string>();

                Maptype = GetMapType(service.Instance.GetSetting(SettingKeys.MapType).GetValue<string>());
            }
        }

        internal String GoogleMapsKey { get; }

        internal bool Traffic { get; }

        internal bool Tilt { get; }

        internal bool Route { get; }

        internal bool ZoomControl { get; }

        internal bool ZoomOnAddress { get; }

        internal string Maptype { get; }

        internal string Home { get; }

        internal int ZoomLevel { get; }

        private string GetMapType(string settingValue)
        {
            switch (settingValue)
            {
                case "Hybrid":
                    return "HYBRID";
                case "Terrain":
                    return "TERRAIN";
                case "Satellit":
                    return "SATELLITE";
                default:
                    return "ROADMAP";
            }
        }
    }
}