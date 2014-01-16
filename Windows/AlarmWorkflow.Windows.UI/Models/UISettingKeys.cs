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

namespace AlarmWorkflow.Windows.UI.Models
{
    static class UISettingKeys
    {
        internal static readonly SettingKey OperationViewerKey = SettingKey.Create("UIConfiguration", "OperationViewer");
        internal static readonly SettingKey FullscreenOnAlarmKey = SettingKey.Create("UIConfiguration", "FullscreenOnAlarm");
        internal static readonly SettingKey AcknowledgeOperationKeyKey = SettingKey.Create("UIConfiguration", "AcknowledgeOperationKey");
        internal static readonly SettingKey AOAIsEnabledKey = SettingKey.Create("UIConfiguration", "AOA.IsEnabled");
        internal static readonly SettingKey AOAMaxAgeKey = SettingKey.Create("UIConfiguration", "AOA.MaxAge");
        internal static readonly SettingKey AvoidScreensaverKey = SettingKey.Create("UIConfiguration", "AvoidScreensaver");
        internal static readonly SettingKey MaxAlarmsInUIKey = SettingKey.Create("UIConfiguration", "MaxAlarmsInUI");
        internal static readonly SettingKey JobsConfigurationKey = SettingKey.Create("UIConfiguration", "JobsConfiguration");
        internal static readonly SettingKey IdleJobsConfigurationKey = SettingKey.Create("UIConfiguration", "IdleJobsConfiguration");
        internal static readonly SettingKey SwitchAlarmsKey = SettingKey.Create("UIConfiguration", "SwitchAlarms");
        internal static readonly SettingKey SwitchTimeKey = SettingKey.Create("UIConfiguration", "SwitchTime");
    }
}
