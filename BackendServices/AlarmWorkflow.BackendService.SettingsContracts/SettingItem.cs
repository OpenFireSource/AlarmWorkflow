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
using System.Diagnostics;
using System.Runtime.Serialization;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.BackendService.SettingsContracts
{
    /// <summary>
    /// Represents a single setting that was read from the configuration and may have a custom value defined.
    /// </summary>
    [DataContract()]
    [DebuggerDisplay("Name = {Name}, Value = {Value} (is user defined = {IsUserValue}, is default = {IsDefault})")]
    public sealed class SettingItem : IProxyType<string>
    {
        #region Fields

        private bool _valueCached;
        private object _value;

        [DataMember()]
        private string _valueSerialized;
        [DataMember()]
        private string _defaultValueSerialized;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier that this setting is associated with.
        /// </summary>
        [DataMember()]
        public string Identifier { get; private set; }
        /// <summary>
        /// Gets the name of this setting.
        /// </summary>
        [DataMember()]
        public string Name { get; private set; }
        /// <summary>
        /// Gets/sets the current value of this setting.
        /// </summary>
        public object Value
        {
            get { return GetOrCacheValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// Gets whether or not the value of this setting is the default value.
        /// </summary>
        public bool IsDefault
        {
            get { return string.Equals(_valueSerialized, _defaultValueSerialized, StringComparison.Ordinal); }
        }
        /// <summary>
        /// Gets whether or not this setting represents a user-set value.
        /// </summary>
        [DataMember()]
        public bool IsUserValue { get; private set; }
        /// <summary>
        /// Gets the type of this setting.
        /// </summary>
        public Type SettingType { get; private set; }
        /// <summary>
        /// Gets/sets the assembly qualified name (AQN) of the <see cref="SettingType"/> property.
        /// This is necessary because System.Type is abstract and its implementation is not usable over WCF!
        /// </summary>
        [DataMember()]
        internal string SettingTypeAqn
        {
            get { return SettingType.AssemblyQualifiedName; }
            set { SettingType = Type.GetType(value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="SettingItem"/> class from being created.
        /// </summary>
        private SettingItem()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingItem"/> class.
        /// </summary>
        /// <param name="identifier">The identifier that this setting is associated with.</param>
        /// <param name="name">The name of this setting.</param>
        /// <param name="defaultValue">The default value of this setting.</param>
        /// <param name="type">The type of the setting.</param>
        internal SettingItem(string identifier, string name, string defaultValue, Type type)
            : this()
        {
            Assertions.AssertNotEmpty(identifier, "identifier");
            Assertions.AssertNotEmpty(name, "name");
            Assertions.AssertNotNull(type, "type");

            this.Identifier = identifier;
            this.Name = name;
            this.SettingType = type;
            _defaultValueSerialized = defaultValue;
        }

        #endregion

        #region Methods

        private object GetOrCacheValue()
        {
            if (!_valueCached)
            {
                if (!IsUserValue)
                {
                    return StringSettingConvertibleTools.ConvertFromSetting(this.SettingType, _defaultValueSerialized);
                }

                _value = StringSettingConvertibleTools.ConvertFromSetting(this.SettingType, _valueSerialized);
                _valueCached = true;
            }
            return _value;
        }

        /// <summary>
        /// Returns the value of this setting casted to its desired type.
        /// </summary>
        /// <typeparam name="T">The type to cast the setting to. If this type is a type that implements <see cref="T:IStringSettingConvertible"/>,
        /// an automatic conversion as defined in that type will be performed.</typeparam>
        /// <returns>The value of this setting casted to its desired type. If the value is null then the default value for <typeparamref name="T"/> is returned.</returns>
        public T GetValue<T>()
        {
            return (T)Value;
        }

        private void SetValue(object value)
        {
            // Only set the value if it the types match
            if (value != null && value.GetType().IsSubclassOf(SettingType))
            {
                return;
            }
            if (value == this.Value)
            {
                return;
            }

            string valueToSave = null;
            if (!StringSettingConvertibleTools.ConvertBack(value, out valueToSave))
            {
                valueToSave = Convert.ToString(value);
            }

            SetSerializedValueAndInvalidateCache(valueToSave);
            IsUserValue = true;
        }

        private void SetSerializedValueAndInvalidateCache(string value)
        {
            _valueSerialized = value;
            _valueCached = false;
            _value = null;
        }

        /// <summary>
        /// Sets the value of this setting using a string-representation.
        /// </summary>
        /// <param name="value">The string-value.</param>
        internal void ApplyUserValue(string value)
        {
            SetSerializedValueAndInvalidateCache(value);
            this.IsUserValue = true;
        }

        /// <summary>
        /// Resets the setting to its default value.
        /// </summary>
        public void ResetValue()
        {
            SetSerializedValueAndInvalidateCache(_defaultValueSerialized);
        }

        /// <summary>
        /// Returns whether or not a given <see cref="SettingItem"/> can have a null-value.
        /// </summary>
        /// <param name="settingItem"></param>
        /// <returns></returns>
        public static bool CanBeNull(SettingItem settingItem)
        {
            return !settingItem.SettingType.IsValueType;
        }

        /// <summary>
        /// Returns a string containing the identifier and the name of this setting in its universal format.
        /// </summary>
        /// <returns>A string containing the identifier and the name of this setting in its universal format.</returns>
        public override string ToString()
        {
            return this.Identifier + "." + this.Name;
        }

        #endregion

        #region IProxyType<string> Members

        string IProxyType<string>.ProxiedValue
        {
            get
            {
                if (IsDefault || !IsUserValue)
                {
                    return _defaultValueSerialized;
                }
                return _valueSerialized;
            }
            set { throw new NotSupportedException(); }
        }

        #endregion
    }
}