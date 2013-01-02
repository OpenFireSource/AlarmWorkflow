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

        #endregion

        #region Methods

        /// <summary>
        /// Returns the display name of the given type, or the type name itself if no InformationAttribute was specified.
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
        /// Returns the description of the given type, or nothing itself if no InformationAttribute was specified.
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
