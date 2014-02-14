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

using System.Collections.ObjectModel;
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.Engine
{
    class EngineServiceInternal : InternalServiceBase, IEngineServiceInternal
    {
        #region Fields

        private AlarmWorkflowEngine _engine;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineServiceInternal"/> class.
        /// </summary>
        public EngineServiceInternal()
            : base()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when the parent service is iterating through all services to signal them they can start.
        /// Overridden to start the engine immediately.
        /// </summary>
        public override void OnStart()
        {
            base.OnStart();

            ISettingsServiceInternal settings = this.ServiceProvider.GetService<ISettingsServiceInternal>();

            Configuration configuration = LoadConfiguration(settings);

            _engine = new AlarmWorkflowEngine(configuration, this.ServiceProvider, settings);
            _engine.Start();
        }

        private static Configuration LoadConfiguration(ISettingsServiceInternal settings)
        {
            Configuration configuration = new Configuration();
            configuration.EnabledAlarmSources = new ReadOnlyCollection<string>(settings.GetSetting(SettingKeys.AlarmSourcesConfigurationKey).GetValue<ExportConfiguration>().GetEnabledExports());
            return configuration;
        }

        /// <summary>
        /// Overridden to stop the engine.
        /// </summary>
        protected override void DisposeCore()
        {
            base.DisposeCore();

            if (_engine != null)
            {
                _engine.Stop();
                _engine = null;
            }
        }

        #endregion
    }
}
