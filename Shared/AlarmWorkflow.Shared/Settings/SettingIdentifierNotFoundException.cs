using System;

namespace AlarmWorkflow.Shared.Settings
{
    /// <summary>
    /// Represents an exception that is thrown when a setting identifier was not found.
    /// </summary>
    [Serializable()]
    public class SettingIdentifierNotFoundException : Exception
    {
        #region Properties

        /// <summary>
        /// Gets the name of the identifier that was not found.
        /// </summary>
        public string IdentifierName { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="SettingIdentifierNotFoundException"/> class from being created.
        /// </summary>
        private SettingIdentifierNotFoundException()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingIdentifierNotFoundException"/> class.
        /// </summary>
        /// <param name="identifierName">The name of the identifier that was not found.</param>
        public SettingIdentifierNotFoundException(string identifierName)
            : base(string.Format(Properties.Resources.SettingIdentifierNotFoundExceptionMessage, identifierName))
        {
            this.IdentifierName = identifierName;
        }

        #endregion
    }
}
