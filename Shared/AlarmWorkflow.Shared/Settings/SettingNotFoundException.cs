using System;

namespace AlarmWorkflow.Shared.Settings
{
    /// <summary>
    /// Represents an exception that is thrown when a setting with a specific name was not found.
    /// </summary>
    [Serializable()]
    public class SettingNotFoundException : Exception
    {
        #region Properties

        /// <summary>
        /// Gets the name of the setting that was not found.
        /// </summary>
        public string SettingName { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="SettingNotFoundException"/> class from being created.
        /// </summary>
        private SettingNotFoundException()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingNotFoundException"/> class.
        /// </summary>
        /// <param name="settingName">The name of the setting that was not found.</param>
        public SettingNotFoundException(string settingName)
            : base(string.Format(Properties.Resources.SettingNotFoundExceptionMessage, settingName))
        {
            this.SettingName = settingName;
        }

        #endregion
    }
}
