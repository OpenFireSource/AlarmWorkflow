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

namespace AlarmWorkflow.Shared.Settings
{
    /// <summary>
    /// Provides a comparer for <see cref="SettingKey"/>s
    /// </summary>
    public class SettingKeyComparer : IEqualityComparer<SettingKey> {

        #region Implementation of IEqualityComparer<in SettingKey>

        /// <summary>
        /// Determines whether the specified <see cref="SettingKey"/>s are equal.
        /// </summary>
        /// <param name="x">The first <see cref="SettingKey"/> to compare.</param>
        /// <param name="y">The second <see cref="SettingKey"/> to compare.</param>
        /// <returns><c>true</c> if the specified objects are equal; otherwise,<c>false.</c></returns>
        public bool Equals(SettingKey x, SettingKey y)
        {
            return x.Name == y.Name && x.Identifier == y.Identifier;
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="SettingKey"/>.
        /// </summary>
        /// <param name="obj">The <see cref="SettingKey"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(SettingKey obj)
        {
            int identifier = obj.Identifier == null ? 0 : obj.Identifier.GetHashCode();
            int name = obj.Name == null ? 0 : obj.Name.GetHashCode();

            return identifier ^ name;
        
        }

        #endregion
    }
}