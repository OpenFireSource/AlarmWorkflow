using System;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.SettingsContracts
{
    /// <summary>
    /// Defines the properties for a setting item
    /// </summary>
    public interface ISettingItem : IProxyType<string>
    {
        /// <summary>
        /// Gets the identifier that this setting is associated with.
        /// </summary>
        string Identifier { get; }
        /// <summary>
        /// Gets whether or not the value of this setting is the default value.
        /// </summary>
        bool IsDefault { get; }
        /// <summary>
        /// Gets whether or not this setting represents a user-set value.
        /// </summary>
        bool IsUserValue { get; }
        /// <summary>
        /// Gets the name of this setting.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the type of this setting.
        /// </summary>
        Type SettingType { get; }
        /// <summary>
        /// Gets/sets the current value of this setting.
        /// </summary>
        object Value { get; set; }
        /// <summary>
        /// Returns the value of this setting casted to its desired type.
        /// </summary>
        /// <typeparam name="T">The type to cast the setting to. If this type is a type that implements <see cref="T:IStringSettingConvertible"/>,
        /// an automatic conversion as defined in that type will be performed.</typeparam>
        /// <returns>The value of this setting casted to its desired type. If the value is null then the default value for <typeparamref name="T"/> is returned.</returns>
        T GetValue<T>();
        /// <summary>
        /// Resets the setting to its default value.
        /// </summary>
        void ResetValue();
        /// <summary>
        /// Returns a string containing the identifier and the name of this setting in its universal format.
        /// </summary>
        /// <returns>A string containing the identifier and the name of this setting in its universal format.</returns>
        string ToString();
    }
}