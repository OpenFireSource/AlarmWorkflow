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
    /// Defines a means for a type that can be converted from a setting value and back.
    /// </summary>
    public interface IStringSettingConvertible
    {
        /// <summary>
        /// Converts the value from the setting and applies it to this instance.
        /// </summary>
        /// <param name="settingValue">The value from the setting.</param>
        void Convert(string settingValue);
        /// <summary>
        /// Converts this instance back into the setting value.
        /// </summary>
        /// <returns>The string value for the setting.</returns>
        string ConvertBack();
    }
}