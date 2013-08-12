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
using System.Linq;
using System.Reflection;

namespace AlarmWorkflow.Shared.Settings
{
    /// <summary>
    /// Contains helper methods that are used for conversion of <see cref="IStringSettingConvertible"/>-objects.
    /// </summary>
    public static class StringSettingConvertibleTools
    {
        /// <summary>
        /// Takes an <see cref="Object"/> which represents the setting value and tries to return its really value
        /// using a conversion to <see cref="IStringSettingConvertible"/>. If that failed then the value is returned as-is.
        /// </summary>
        /// <typeparam name="T">The expected true type of the setting.</typeparam>
        /// <param name="value">The setting value.</param>
        /// <returns></returns>
        public static T ConvertFromSetting<T>(object value)
        {
            if (value == null)
            {
                return default(T);
            }

            // If the value is a string
            if (value is string && typeof(T).GetInterface(typeof(IStringSettingConvertible).Name) != null)
            {
                // Look for an empty constructor, even if it is private (only then we can instantiate this type)
                if (typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any(ci => ci.GetParameters().Length == 0))
                {
                    IStringSettingConvertible convertible = (IStringSettingConvertible)Activator.CreateInstance(typeof(T), true);
                    convertible.Convert((string)value);
                    return (T)convertible;
                }
            }

            return (T)value;
        }

        /// <summary>
        /// Tries to cast the value to <see cref="IStringSettingConvertible"/> and performs the ConvertBack()-method if successful.
        /// Otherwise returns false and does nothing.
        /// </summary>
        /// <param name="value">The value to convert. May be of type <see cref="IStringSettingConvertible"/>.</param>
        /// <param name="converted">If conversion succeeded, this parameter contains the converted value. Otherwise contains null.</param>
        /// <returns>A boolean value indicating whether or not conversion was successful.</returns>
        public static bool ConvertBack(object value, out object converted)
        {
            IStringSettingConvertible convertible = value as IStringSettingConvertible;
            if (convertible != null)
            {
                converted = convertible.ConvertBack();
                return true;
            }

            converted = null;
            return false;
        }
    }
}