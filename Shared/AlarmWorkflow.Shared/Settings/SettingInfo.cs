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

using System.Diagnostics;

namespace AlarmWorkflow.Shared.Settings
{
    /// <summary>
    /// Display configuration for a single setting.
    /// </summary>
    [DebuggerDisplay("Name = {Name}, Category = {Category}")]
    public class SettingInfo
    {
        #region Properties

        /// <summary>
        /// Gets/sets the name of the setting.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets/sets the category of the setting.
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Gets/sets the display text of the setting. If there is no display text, the Name will be used.
        /// </summary>
        public string DisplayText { get; set; }
        /// <summary>
        /// Gets/sets the description of the setting.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Gets/sets the order of the setting.
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Gets/sets the name of the editor of the setting. If this is empty, the default editor will be used.
        /// </summary>
        public string Editor { get; set; }
        /// <summary>
        /// Gets/sets an optional parameter for the editor of the setting.
        /// </summary>
        public string EditorParameter { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingInfo"/> class.
        /// </summary>
        public SettingInfo()
        {

        }

        #endregion
    }
}