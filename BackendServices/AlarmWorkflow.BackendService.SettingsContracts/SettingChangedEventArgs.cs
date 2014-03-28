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
using System.Collections.Generic;
using System.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.BackendService.SettingsContracts
{
    /// <summary>
    /// Represents the event that one or more settings have changed their value.
    /// </summary>
    public class SettingChangedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="SettingKey"/>-instances describing the identifiers and names of the settings that have changed.
        /// </summary>
        public IEnumerable<SettingKey> Keys { get; private set; }

        #endregion

        #region Constructors

        private SettingChangedEventArgs()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingChangedEventArgs"/> class.
        /// </summary>
        /// <param name="keys">The <see cref="SettingKey"/>, representing the settings that have changed.</param>
        public SettingChangedEventArgs(IEnumerable<SettingKey> keys)
            : this()
        {
            Assertions.AssertNotNull(keys, "keys");

            Keys = keys;
        }

        #endregion
    }
}
