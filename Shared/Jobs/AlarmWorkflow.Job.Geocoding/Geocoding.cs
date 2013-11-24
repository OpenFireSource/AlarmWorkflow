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
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Job.Geocoding
{
    /// <summary>
    /// A job doing geocoding based on the given provider. The job is executed only when no coordinates are already defined.
    /// </summary>
    [Export("Geocoding", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    public class Geocoding : IJob
    {
        #region Fields

        private IGeoCoder _provider;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {

        }

        #endregion

        #region IJob Members

        bool IJob.Initialize()
        {
            _provider = ExportedTypeLibrary.Import<IGeoCoder>(SettingsManager.Instance.GetSetting("Geocoding", "Provider").GetString());
            if (_provider.ApiKeyRequired)
            {
                string apikey = SettingsManager.Instance.GetSetting("Geocoding", "ApiKey").GetString();
                _provider.ApiKey = apikey;
            }
            return true;
        }

        void IJob.Execute(IJobContext context, Operation operation)
        {
            // This is a Pre-Job. Thus it only has to run right after the operation has surfaced and before being stored.
            if (context.Phase == JobPhase.OnOperationSurfaced)
            {
                //Don't overwrite exisiting coordinates
                if (operation.Einsatzort.HasGeoCoordinates)
                {
                    return;
                }
                GeocoderLocation geocoderLocation = _provider.GeoCode(operation.Einsatzort);
                if (geocoderLocation != null)
                {
                    //The most of the widgets and so need the "english-format" (point instead of comma)!
                    operation.Einsatzort.GeoLongitude = geocoderLocation.Longitude.ToString().Replace(',','.');;
                    operation.Einsatzort.GeoLatitude = geocoderLocation.Latitude.ToString().Replace(',', '.'); ;
                }
                else
                {
                    Logger.Instance.LogFormat(LogType.Error, this,"No coordinats found by geocoding service.");
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
