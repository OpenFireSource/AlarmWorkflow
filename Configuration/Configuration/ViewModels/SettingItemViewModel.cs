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
using System.Windows;
using System.Windows.Input;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.ConfigurationContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    [DebuggerDisplay("Setting = {SettingDescriptor.Identifier}/{SettingDescriptor.SettingItem.Name}")]
    class SettingItemViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets the setting descriptor for this item.
        /// </summary>
        public SettingInfo Info { get; private set; }
        /// <summary>
        /// Gets the actual setting item containing the value.
        /// </summary>
        public SettingItem Setting { get; private set; }
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
                if (Info == null || string.IsNullOrWhiteSpace(Info.DisplayText))
                {
                    return Info.Name;
                }
                return Info.DisplayText;
            }
        }
        /// <summary>
        /// Gets the description of this setting.
        /// </summary>
        public string Description
        {
            get
            {
                if (Info == null || string.IsNullOrWhiteSpace(Info.Description))
                {
                    return "(Keine Beschreibung vorhanden)";
                }
                return Info.Description;
            }
        }

        private string Editor
        {
            get
            {
                if (Info == null || string.IsNullOrWhiteSpace(Info.Editor))
                {
                    return null;
                }
                return Info.Editor;
            }
        }

        private string EditorParameter
        {
            get
            {
                if (Info == null || string.IsNullOrWhiteSpace(Info.EditorParameter))
                {
                    return null;
                }
                return Info.EditorParameter;
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

            this.Setting.ResetValue();
            this.TypeEditor.Value = this.Setting.Value;

            OnPropertyChanged("TypeEditor");
            OnPropertyChanged("TypeEditor.Value");
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingItemViewModel"/> class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="setting"></param>
        public SettingItemViewModel(SettingInfo info, SettingItem setting)
        {
            Assertions.AssertNotNull(info, "info");
            Assertions.AssertNotNull(setting, "setting");

            Info = info;
            Setting = setting;

            // Find out editor
            string editorName = Editor;
            if (string.IsNullOrWhiteSpace(editorName))
            {
                editorName = Setting.SettingType.FullName;
            }

            this.TypeEditor = TypeEditors.TypeEditorCache.CreateTypeEditor(editorName);
            this.TypeEditor.Initialize(this.EditorParameter);
            try
            {
                this.TypeEditor.Value = Setting.Value;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.SettingItemViewModelSetSettingError, Info.Name, Info.Identifier);
                Logger.Instance.LogException(this, ex);

                Setting.ResetValue();
                this.TypeEditor.Value = Setting.Value;
            }
        }

        #endregion
    }
}