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

namespace AlarmWorkflow.Shared.Settings
{
    /// <summary>
    /// Describes one <see cref="SettingItem"/> in more detail.
    /// </summary>
    public sealed class SettingDescriptor
    {
        #region Properties

        /// <summary>
        /// Gets the identifier this setting belongs to.
        /// </summary>
        public string Identifier { get; private set; }
        /// <summary>
        /// Gets the setting item. This is a cloned value of the original <see cref="SettingItem"/>.
        /// </summary>
        public SettingItem SettingItem { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingDescriptor"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="settingItem">The setting item.</param>
        internal SettingDescriptor(string identifier, SettingItem settingItem)
        {
            this.Identifier = identifier;
            this.SettingItem = (SettingItem)settingItem.Clone();
        }

        #endregion
    }
}