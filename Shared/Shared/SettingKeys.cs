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

namespace AlarmWorkflow.Shared
{
    /// <summary>
    /// Represents the individual setting items in this assembly.
    /// </summary>
    public static class SettingKeys
    {
        /// <summary>
        /// Identifies the "FD.Name" setting.
        /// </summary>
        public static readonly SettingKey FDName = SettingKey.Create("Shared", "FD.Name");
        /// <summary>
        /// Identifies the "FD.City" setting.
        /// </summary>
        public static readonly SettingKey FDCity = SettingKey.Create("Shared", "FD.City");
        /// <summary>
        /// Identifies the "FD.ZipCode" setting.
        /// </summary>
        public static readonly SettingKey FDZipCode = SettingKey.Create("Shared", "FD.ZipCode");
        /// <summary>
        /// Identifies the "FD.Street" setting.
        /// </summary>
        public static readonly SettingKey FDStreet = SettingKey.Create("Shared", "FD.Street");
        /// <summary>
        /// Identifies the "FD.StreetNumber" setting.
        /// </summary>
        public static readonly SettingKey FDStreetNumber = SettingKey.Create("Shared", "FD.StreetNumber");
        /// <summary>
        /// Identifies the "Replace Dictionary" setting.
        /// </summary>
        public static readonly SettingKey ReplaceDictionary = SettingKey.Create("Shared", "ReplaceDictionary");
        /// <summary>
        /// Identifies the "Printing queues configuration" setting.
        /// </summary>
        public static readonly SettingKey PrintingQueuesConfiguration = SettingKey.Create("Shared", "PrintingQueuesConfiguration");
    }
}
