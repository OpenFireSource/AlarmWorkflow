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
    /// Represents an exception that is thrown when a setting with a specific name was not found.
    /// </summary>
    [Serializable()]
    public class SettingNotFoundException : Exception
    {
        #region Properties

        /// <summary>
        /// Gets the name of the setting that was not found.
        /// </summary>
        public string SettingName { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="SettingNotFoundException"/> class from being created.
        /// </summary>
        private SettingNotFoundException()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingNotFoundException"/> class.
        /// </summary>
        /// <param name="settingName">The name of the setting that was not found.</param>
        public SettingNotFoundException(string settingName)
            : base(string.Format(Properties.Resources.SettingNotFoundExceptionMessage, settingName))
        {
            this.SettingName = settingName;
        }

        #endregion
    }
}