using System;

namespace AlarmWorkflow.Shared.Settings
{
    /// <summary>
    /// Describes one <see cref="SettingItem"/> in more detail.
    /// </summary>
    public sealed class SettingDescriptor
    {
        #region Properties

        /// <summary>
        /// Gets the identifier this setting belongs to.
        /// </summary>
        public string Identifier { get; private set; }
        /// <summary>
        /// Gets the setting item. This is a cloned value of the original <see cref="SettingItem"/>.
        /// </summary>
        public SettingItem SettingItem { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingDescriptor"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="settingItem">The setting item.</param>
        internal SettingDescriptor(string identifier, SettingItem settingItem)
        {
            this.Identifier = identifier;
            this.SettingItem = (SettingItem)settingItem.Clone();
        }

        #endregion
    }
}
