using System;
using System.Collections.Generic;
using System.Diagnostics;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.ConfigurationContracts;
using AlarmWorkflow.Windows.UI.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    [DebuggerDisplay("Setting = {SettingDescriptor.Identifier}/{SettingDescriptor.SettingItem.Name}")]
    class SettingItemViewModel : ViewModelBase
    {
        #region Static

        private static readonly Dictionary<string, Type> TypeEditors;

        static SettingItemViewModel()
        {
            TypeEditors = new Dictionary<string, Type>();
            TypeEditors[""] = typeof(TypeEditors.DefaultTypeEditor);
            TypeEditors["System.String"] = typeof(TypeEditors.StringTypeEditor);
            TypeEditors["System.Int32"] = typeof(TypeEditors.Int32TypeEditor);
            TypeEditors["System.Boolean"] = typeof(TypeEditors.BooleanTypeEditor);
            TypeEditors["StringArrayEditor"] = typeof(TypeEditors.StringArrayTypeEditor);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the setting descriptor for this item.
        /// </summary>
        public SettingDescriptor SettingDescriptor { get; private set; }
        /// <summary>
        /// Gets the type editor for this setting item.
        /// </summary>
        public ITypeEditor TypeEditor { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingItemViewModel"/> class.
        /// </summary>
        /// <param name="settingDescriptor">The setting descriptor.</param>
        public SettingItemViewModel(SettingDescriptor settingDescriptor)
        {
            this.SettingDescriptor = settingDescriptor;

            // Find out editor
            string editorName = settingDescriptor.SettingItem.SettingType.FullName;
            if (!string.IsNullOrWhiteSpace(settingDescriptor.SettingItem.EditorName))
            {
                editorName = settingDescriptor.SettingItem.EditorName;
            }

            if (!TypeEditors.ContainsKey(editorName))
            {
                editorName = "";
            }

            this.TypeEditor = (ITypeEditor)Activator.CreateInstance(TypeEditors[editorName]);
            this.TypeEditor.Value = settingDescriptor.SettingItem.Value;
        }

        #endregion
    }
}
