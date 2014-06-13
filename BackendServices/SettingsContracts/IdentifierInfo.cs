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
using System.Diagnostics;
using System.Runtime.Serialization;

namespace AlarmWorkflow.BackendService.SettingsContracts
{
    /// <summary>
    /// Display configuration for an identifier section.
    /// </summary>
    [DataContract()]
    [DebuggerDisplay("Name = {Name}, Settings count = {Settings.Count}")]
    public class IdentifierInfo
    {
        #region Properties

        /// <summary>
        /// Gets/sets the name of the identifier section.
        /// </summary>
        [DataMember()]
        public string Name { get; set; }
        /// <summary>
        /// Gets/sets the display text of the identifier section. If there is none, the Name will be used.
        /// </summary>
        [DataMember()]
        public string DisplayText { get; set; }
        /// <summary>
        /// Gets/sets the description of the identifier section.
        /// </summary>
        [DataMember()]
        public string Description { get; set; }
        /// <summary>
        /// Gets/sets the order of the identifier section.
        /// </summary>
        [DataMember()]
        public int Order { get; set; }
        /// <summary>
        /// Gets/sets the identifier of the parent section.
        /// Use null for no parent section.
        /// </summary>
        [DataMember()]
        public string Parent { get; set; }

        /// <summary>
        /// Gets/sets the settings that are contained in this identifier section.
        /// </summary>
        [DataMember()]
        public List<SettingInfo> Settings { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierInfo"/> class.
        /// </summary>
        public IdentifierInfo()
        {
            Settings = new List<SettingInfo>();
        }

        #endregion
    }
}