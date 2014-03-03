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
using System.Collections.Generic;
using System.Windows;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.ConfigurationContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    /// <summary>
    /// Handles the asynchronous saving of setting items and keeping the user updated about the progress.
    /// </summary>
    class SaveSettingsTaskCommand : BackgroundTaskCommand
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveSettingsTaskCommand"/> class.
        /// </summary>
        /// <param name="parent">The parent ViewModel.</param>
        public SaveSettingsTaskCommand(MainViewModel parent)
            : base()
        {
            Assertions.AssertNotNull(parent, "parent");
            Parameters.Add("Parent", parent);
        }

        #endregion

        #region Methods

        private MainViewModel GetParent()
        {
            return (MainViewModel)Parameters["Parent"];
        }

        /// <summary>
        /// Execution is only possible if we are connected and the task is not running.
        /// </summary>
        /// <param name="parameter">An optional parameter for the command.</param>
        /// <returns>Whether or not the command can be executed right now.</returns>
        protected override bool CanExecute(object parameter)
        {
            return GetParent().IsConnected && base.CanExecute(parameter);
        }

        /// <summary>
        /// Stores all settings at the service.
        /// </summary>
        /// <param name="parameter">An optional parameter for the command.</param>
        protected override void Execute(object parameter)
        {
            List<Dictionary<SettingKey, SettingItem>> settings = new List<Dictionary<SettingKey, SettingItem>>();
            int iFailedSettings = 0;

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                foreach (GroupedSectionViewModel gsvm in this.GetParent().GetAllSections())
                {
                    SectionViewModel svm = gsvm.Section;
                    if (svm == null)
                    {
                        continue;
                    }

                    Dictionary<SettingKey, SettingItem> sectionSettings = new Dictionary<SettingKey, SettingItem>();
                    settings.Add(sectionSettings);

                    foreach (CategoryViewModel cvm in svm.CategoryItems)
                    {
                        foreach (SettingItemViewModel sivm in cvm.SettingItems)
                        {
                            object value = null;
                            try
                            {
                                value = sivm.TypeEditor.Value;

                                sivm.Setting.Value = value;
                                sectionSettings.Add(sivm.Info.CreateSettingKey(), sivm.Setting);
                            }
                            catch (Exception ex)
                            {
                                string exMessage = ex.Message;
                                string exHint = Properties.Resources.SettingSaveError_DefaultHints;

                                ValueException vex = ex as ValueException;
                                if (vex != null)
                                {
                                    exHint = vex.Hint;
                                }

                                Logger.Instance.LogException(this, ex);

                                string message = string.Format(Properties.Resources.SettingSaveError, sivm.DisplayText, svm.DisplayText, exMessage, exHint);
                                MessageBox.Show(message, Properties.Resources.SettingSaveError_Title, MessageBoxButton.OK, MessageBoxImage.Error);
                                iFailedSettings++;
                            }
                        }
                    }
                }
            }));

            iFailedSettings += SaveSettingItems(settings);

            string boxMessage = null;
            MessageBoxImage boxImage = MessageBoxImage.Information;
            if (iFailedSettings == 0)
            {
                boxMessage = Properties.Resources.SavingSettingsSuccess;
            }
            else
            {
                boxMessage = Properties.Resources.SavingSettingsWithErrors;
                boxImage = MessageBoxImage.Warning;
            }
            MessageBox.Show(boxMessage, Properties.Resources.SettingSaveFinished_Title, MessageBoxButton.OK, boxImage);
        }

        private int SaveSettingItems(List<Dictionary<SettingKey, SettingItem>> settings)
        {
            int failedSaves = 0;

            using (var service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(new SettingsServiceCallback()))
            {
                int i = 0;
                foreach (var section in settings)
                {
                    try
                    {
                        service.Instance.SetSettings(section);
                    }
                    catch (Exception ex)
                    {
                        failedSaves++;
                        Logger.Instance.LogException(this, ex);
                    }

                    i++;

                    this.Progress = (int)((i * 100) / settings.Count);
                    this.SetProgressText(Properties.Resources.SaveSettingStatusText, i, settings.Count);
                }
            }

            return failedSaves;
        }

        #endregion
    }
}
