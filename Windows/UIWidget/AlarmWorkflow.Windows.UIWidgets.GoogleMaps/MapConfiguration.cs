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

using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.SettingsContracts;
using SharedSettingKeys = AlarmWorkflow.Shared.SettingKeys;

namespace AlarmWorkflow.Windows.UIWidgets.GoogleMaps
{
    class MapConfiguration
    {
        internal MapConfiguration()
        {
            using (var service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(new SettingsServiceCallback()))
            {
                Traffic = service.Instance.GetSetting(SettingKeys.Traffic).GetValue<bool>();
                Tilt = service.Instance.GetSetting(SettingKeys.Tilt).GetValue<bool>();
                Route = service.Instance.GetSetting(SettingKeys.Route).GetValue<bool>();
                ZoomControl = service.Instance.GetSetting(SettingKeys.ZoomControl).GetValue<bool>();
                ZoomLevel = service.Instance.GetSetting(SettingKeys.ZoomLevel).GetValue<int>();
                Home = service.Instance.GetSetting(SharedSettingKeys.FDStreet).GetValue<string>() + " " +
                       service.Instance.GetSetting(SharedSettingKeys.FDStreetNumber).GetValue<string>() + " " +
                       service.Instance.GetSetting(SharedSettingKeys.FDZipCode).GetValue<string>() + " " +
                       service.Instance.GetSetting(SharedSettingKeys.FDCity).GetValue<string>();

                Maptype = GetMapType(service.Instance.GetSetting(SettingKeys.MapType).GetValue<string>());
            }
        }

        internal bool Traffic { get; private set; }

        internal bool Tilt { get; private set; }

        internal bool Route { get; private set; }

        internal bool ZoomControl { get; private set; }

        internal bool RouteDescription { get; private set; }

        internal string Maptype { get; private set; }

        internal string Home { get; private set; }

        internal int ZoomLevel { get; private set; }

        private string GetMapType(string settingValue)
        {
            switch (settingValue)
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