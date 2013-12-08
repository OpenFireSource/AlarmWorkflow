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

using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Windows.UIWidgets.GoogleMaps
{
    static class SettingKeys
    {
        internal static readonly SettingKey Traffic = SettingKey.Create("GoogleMapsWidget", "Traffic");
        internal static readonly SettingKey Tilt = SettingKey.Create("GoogleMapsWidget", "Tilt");
        internal static readonly SettingKey Route = SettingKey.Create("GoogleMapsWidget", "Route");
        internal static readonly SettingKey ZoomControl = SettingKey.Create("GoogleMapsWidget", "ZoomControl");
        internal static readonly SettingKey ZoomLevel = SettingKey.Create("GoogleMapsWidget", "ZoomLevel");
        internal static readonly SettingKey MapType = SettingKey.Create("GoogleMapsWidget", "MapType");
    }
}
