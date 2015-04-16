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
using System.ComponentModel;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Shared.Specialized;

namespace AlarmWorkflow.AlarmSource.Fax
{
    sealed class FaxConfiguration : DisposableObject, INotifyPropertyChanged
    {
        #region Constants

        private const string Identifier = "FaxAlarmSource";

        #endregion

        #region Fields

        private ISettingsServiceInternal _settings;

        #endregion

        #region Properties

        internal string FaxPath
        {
            get { return _settings.GetSetting(FaxSettingKeys.FaxPath).GetValue<string>(); }
        }

        internal string ArchivePath
        {
            get { return _settings.GetSetting(FaxSettingKeys.ArchivePath).GetValue<string>(); }
        }

        internal string AnalysisPath
        {
            get { return _settings.GetSetting(FaxSettingKeys.AnalysisPath).GetValue<string>(); }
        }

        internal string OCRSoftwarePath
        {
            get { return _settings.GetSetting(FaxSettingKeys.OcrPath).GetValue<string>(); }
        }

        internal string AlarmFaxParserAlias
        {
            get { return _settings.GetSetting(FaxSettingKeys.AlarmFaxParserAlias).GetValue<string>(); }
        }

        internal ReplaceDictionary ReplaceDictionary
        {
            get { return _settings.GetSetting(SettingKeys.ReplaceDictionary).GetValue<ReplaceDictionary>(); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FaxConfiguration"/> class.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public FaxConfiguration(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();
            _settings.SettingChanged += _settings_SettingChanged;
        }

        #endregion

        #region Methods

        private void _settings_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            List<string> changedKeys = new List<string>();

            foreach (SettingKey key in e.Keys)
            {
                if (key.Equals(FaxSettingKeys.AlarmFaxParserAlias))
                {
                    changedKeys.Add("AlarmfaxParser");
                }
                else if (key.Equals(FaxSettingKeys.OcrPath))
                {
                    changedKeys.Add("OCR.Path");
                }
                else if ((key.Equals(FaxSettingKeys.FaxPath) || key.Equals(FaxSettingKeys.AnalysisPath) || key.Equals(FaxSettingKeys.ArchivePath)) && !changedKeys.Contains("FaxPaths"))
                {
                    changedKeys.Add("FaxPaths");
                }
            }

            foreach (string key in changedKeys)
            {
                OnPropertyChanged(key);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void DisposeCore()
        {
            _settings.SettingChanged -= _settings_SettingChanged;
            _settings = null;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}