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
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Job.OperationFileExporter
{
    class Configuration : DisposableObject
    {
        #region Fields

        private ISettingsServiceInternal _settings;

        #endregion

        #region Properties

        internal bool CustomTextExportEnabled
        {
            get { return _settings.GetSetting(SettingKeys.CustomTextExportEnabled).GetValue<bool>(); }
        }

        internal string CustomTextDestinationFileName
        {
            get { return _settings.GetSetting(SettingKeys.CustomTextDestinationFileName).GetValue<string>(); }
        }

        internal string CustomTextFormat
        {
            get { return _settings.GetSetting(SettingKeys.CustomTextFormat).GetValue<string>(); }
        }

        internal bool EvaExportEnabled
        {
            get { return _settings.GetSetting(SettingKeys.EVAExportEnabled).GetValue<bool>(); }
        }

        internal string EvaDestinationFileName
        {
            get { return _settings.GetSetting(SettingKeys.EVADestinationFileName).GetValue<string>(); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider to use.</param>
        public Configuration(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void DisposeCore()
        {
            if (_settings != null)
            {
                _settings = null;
            }
        }

        #endregion
    }
}
