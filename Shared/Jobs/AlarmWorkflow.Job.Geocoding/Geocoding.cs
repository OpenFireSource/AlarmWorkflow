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
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.Geocoding
{
    /// <summary>
    /// A job doing geocoding based on the given provider. The job is executed only when no coordinates are already defined.
    /// </summary>
    [Export("Geocoding", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    class Geocoding : IJob
    {
        #region Fields

        private ISettingsServiceInternal _settings;
        private IGeoCoder _provider;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {

        }

        #endregion

        #region IJob Members

        bool IJob.Initialize(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();

            _provider = ExportedTypeLibrary.Import<IGeoCoder>(_settings.GetSetting(SettingKeysJob.Provider).GetValue<string>());

            if (_provider.IsApiKeyRequired)
            {
                _provider.ApiKey = _settings.GetSetting(SettingKeysJob.ApiKey).GetValue<string>();
            }

            return true;
        }

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase == JobPhase.OnOperationSurfaced)
            {
                if (operation.Einsatzort.HasGeoCoordinates)
                {
                    return;
                }

                GeocoderLocation geocoderLocation = _provider.Geocode(operation.Einsatzort);
                if (geocoderLocation != null)
                {
                    //The most of the widgets and so need the "english-format" (point instead of comma)!
                    operation.Einsatzort.GeoLongitude = geocoderLocation.Longitude.ToString().Replace(',', '.');
                    operation.Einsatzort.GeoLatitude = geocoderLocation.Latitude.ToString().Replace(',', '.');
                }
                else
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.NoCoordinatesFoundByGeocodingService);
                }
            }
        }

        bool IJob.IsAsync
        {
            get { return false; }
        }

        #endregion

    }
}
