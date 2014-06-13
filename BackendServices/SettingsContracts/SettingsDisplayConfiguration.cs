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

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AlarmWorkflow.BackendService.SettingsContracts
{
    /// <summary>
    /// Provides information about the settings, to be used in UIs or anyone who needs it.
    /// </summary>
    [DataContract()]
    public sealed class SettingsDisplayConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets a collection of all registered identifiers.
        /// </summary>
        [DataMember()]
        public List<IdentifierInfo> Identifiers { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsDisplayConfiguration"/> class.
        /// </summary>
        public SettingsDisplayConfiguration()
        {
            Identifiers = new List<IdentifierInfo>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tries to find the <see cref="IdentifierInfo"/> for the section by the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IdentifierInfo GetIdentifier(string name)
        {
            return Identifiers.Find(i => i.Name == name);
        }

        /// <summary>
        /// Tries to find the <see cref="SettingInfo"/> for the setting within the given section and name.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public SettingInfo GetSetting(string identifier, string name)
        {
            IdentifierInfo inf = GetIdentifier(identifier);
            if (inf != null)
            {
                return inf.Settings.Find(s => s.Name == name);
            }
            return null;
        }

        #endregion
    }
}