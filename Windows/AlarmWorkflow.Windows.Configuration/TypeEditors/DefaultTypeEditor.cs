using System;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Represents a type editor that is used if no editor has been found for a specific setting, or the EditorName of one setting led to an unknown editor.
    /// </summary>
    class DefaultTypeEditor : ITypeEditor
    {
        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value { get; set; }

        public System.Windows.UIElement Visual { get; private set; }

        #endregion
    }
}
