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
