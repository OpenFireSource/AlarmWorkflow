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
using AlarmWorkflow.AlarmSource.Network.Server;

namespace AlarmWorkflow.AlarmSource.Network
{
    [Export("NetworkAlarmSource", typeof(IAlarmSource))]
    [Information(DisplayName = "ExportAlarmSourceDisplayName", Description = "ExportAlarmSourceDescription")]
    class NetworkAlarmSource : IAlarmSource
    {

        #region Fields

        private ISettingsServiceInternal _settings;
        private AlarmServer _server;

        #endregion

        #region IAlarmSource Members

        void IAlarmSource.Initialize(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();

            int outputPort = _settings.GetSetting(NetworkSettingKeys.OutputPort).GetValue<int>();
            _server = new AlarmServer(this, outputPort);
        }

        internal void PushIncomingAlarm(Operation operation)
        {
            operation.TimestampIncome = DateTime.Now;

            if (operation.Timestamp == null)
            {
                operation.Timestamp = DateTime.Now;
            }

            OnNewAlarm(new AlarmSourceEventArgs(operation));
        }

        /// <summary>
        /// Raised when a new alarm has surfaced and processed for the Engine to handle.
        /// See documentation for further information.
        /// </summary>
        public event EventHandler<AlarmSourceEventArgs> NewAlarm;

        private void OnNewAlarm(AlarmSourceEventArgs e)
        {
            var copy = NewAlarm;
            copy?.Invoke(this, e);
        }

        void IAlarmSource.RunThread()
        {
            _server.Start();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (_server.IsRunning)
            {
                _server.Stop();
            }

            _settings = null;
        }

        #endregion
    }
}
