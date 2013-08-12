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

using System.Diagnostics;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.ConfigurationContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;
using System.Windows.Input;
using System.Windows;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    [DebuggerDisplay("Setting = {SettingDescriptor.Identifier}/{SettingDescriptor.SettingItem.Name}")]
    class SettingItemViewModel : ViewModelBase
    {
        #region Fields

        private SettingInfo _setting;

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
        /// <summary>
        /// Gets the text to display in the UI.
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (_setting == null || string.IsNullOrWhiteSpace(_setting.DisplayText))
                {
                    return SettingDescriptor.SettingItem.Name;
                }
                return _setting.DisplayText;
            }
        }
        /// <summary>
        /// Gets the description of this setting.
        /// </summary>
        public string Description
        {
            get
            {
                if (_setting == null || string.IsNullOrWhiteSpace(_setting.Description))
                {
                    return "(Keine Beschreibung vorhanden)";
                }
                return _setting.Description;
            }
        }

        private string Editor
        {
            get
            {
                if (_setting == null || string.IsNullOrWhiteSpace(_setting.Editor))
                {
                    return null;
                }
                return _setting.Editor;
            }
        }

        private string EditorParameter
        {
            get
            {
                if (_setting == null || string.IsNullOrWhiteSpace(_setting.EditorParameter))
                {
                    return null;
                }
                return _setting.EditorParameter;
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// The ResetSettingCommand command.
        /// </summary>
        public ICommand ResetSettingCommand { get; private set; }

        private void ResetSettingCommand_Execute(object parameter)
        {
            if (MessageBox.Show(Properties.Resources.SettingResetConfirmation, "Bestätigung", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            this.TypeEditor.Value = this.SettingDescriptor.SettingItem.DefaultValue;
            this.SettingDescriptor.SettingItem.ResetValue();

            OnPropertyChanged("TypeEditor");
            OnPropertyChanged("TypeEditor.Value");
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingItemViewModel"/> class.
        /// </summary>
        /// <param name="settingDescriptor">The setting descriptor.</param>
        /// <param name="setting"></param>
        public SettingItemViewModel(SettingDescriptor settingDescriptor, SettingInfo setting)
        {
            _setting = setting;

            this.SettingDescriptor = settingDescriptor;

            // Find out editor
            string editorName = Editor;
            if (string.IsNullOrWhiteSpace(editorName))
            {
                editorName = settingDescriptor.SettingItem.SettingType.FullName;
            }

            this.TypeEditor = TypeEditors.TypeEditorCache.CreateTypeEditor(editorName);
            this.TypeEditor.Initialize(this.EditorParameter);
            try
            {
                this.TypeEditor.Value = settingDescriptor.SettingItem.Value;
            }
            catch (System.Exception)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.SettingItemViewModelSetSettingError, settingDescriptor.SettingItem.Name, settingDescriptor.Identifier);
                this.TypeEditor.Value = settingDescriptor.SettingItem.DefaultValue;
            }
        }

        #endregion
    }
}