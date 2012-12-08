using System;

namespace AlarmWorkflow.Windows.ConfigurationContracts
{
    /// <summary>
    /// Provides detailed information about a type that implements <see cref="ITypeEditor"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ConfigurationTypeEditorAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets the type that this type editor can edit.
        /// </summary>
        public Type SourceType { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="ConfigurationTypeEditorAttribute"/> class from being created.
        /// </summary>
        private ConfigurationTypeEditorAttribute()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationTypeEditorAttribute"/> class.
        /// </summary>
        /// <param name="sourceType">The type that this type editor can edit.</param>
        public ConfigurationTypeEditorAttribute(Type sourceType)
            : this()
        {
            SourceType = sourceType;
        }

        #endregion
    }
}
