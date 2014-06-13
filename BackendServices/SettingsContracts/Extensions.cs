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

namespace AlarmWorkflow.BackendService.SettingsContracts
{
    /// <summary>
    /// Provides some extension methods to ensure legacy usage.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the value of this setting as a string array
        /// by splitting the string at their newline-characters.
        /// </summary>
        /// <returns>The value of this setting as a string array.</returns>
        public static string[] GetStringArray(this SettingItem setting)
        {
            string full = setting.GetValue<string>();
            // For some reason, when saving a setting in the editor,
            // it will insert a "\n" instead of the "\r\n" so we need to make sure that both are covered!
            return full.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
