using System.Diagnostics;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.ConfigurationContracts;
using AlarmWorkflow.Windows.UI.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    [DebuggerDisplay("Setting = {SettingDescriptor.Identifier}/{SettingDescriptor.SettingItem.Name}")]
    class SettingItemViewModel : ViewModelBase
    {
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

            this.TypeEditor = TypeEditors.TypeEditorCache.CreateTypeEditor(editorName);
            this.TypeEditor.Value = settingDescriptor.SettingItem.Value;
        }

        #endregion
    }
}
