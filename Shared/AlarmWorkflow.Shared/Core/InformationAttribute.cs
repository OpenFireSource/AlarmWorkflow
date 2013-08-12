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

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Provides custom information about any program element.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class InformationAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets/sets the display-friendly name of this program element.
        /// This may be the name of a resource defined in the assembly's main resources.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets/sets a description of this program element.
        /// This may be the name of a resource defined in the assembly's main resources.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Gets/sets a custom object that can be used as a tag value, or any other custom data value.
        /// </summary>
        public object Tag { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the display name of the given type, or the type name itself if no <see cref="InformationAttribute"/> was specified.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDisplayName(Type type)
        {
            Assertions.AssertNotNull(type, "type");

            InformationAttribute attribute = GetAttribute(type);
            if (attribute != null)
            {
                string res = type.GetResourceString(attribute.DisplayName);
                return res ?? attribute.DisplayName;
            }

            return type.Name;
        }

        /// <summary>
        /// Returns the description of the given type, or nothing if no <see cref="InformationAttribute"/> was specified.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDescription(Type type)
        {
            Assertions.AssertNotNull(type, "type");

            InformationAttribute attribute = GetAttribute(type);
            if (attribute != null)
            {
                string res = type.GetResourceString(attribute.Description);
                return res ?? attribute.Description;
            }

            return String.Empty;
        }

        /// <summary>
        /// Returns the tag of the given type, or nothing if no <see cref="InformationAttribute"/> was specified.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetTag(Type type)
        {
            Assertions.AssertNotNull(type, "type");

            InformationAttribute attribute = GetAttribute(type);
            if (attribute != null)
            {
                return attribute.Tag;
            }

            return String.Empty;
        }

        private static InformationAttribute GetAttribute(Type type)
        {
            InformationAttribute[] attributes = (InformationAttribute[])type.GetCustomAttributes(typeof(InformationAttribute), false);
            if (attributes.Length == 1)
            {
                InformationAttribute attrib = attributes[0];
                return attrib;
            }
            return null;
        }

        #endregion
    }
}