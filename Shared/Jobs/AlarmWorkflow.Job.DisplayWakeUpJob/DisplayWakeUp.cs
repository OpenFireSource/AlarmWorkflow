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
using System.IO;
using System.Net;
using System.Threading;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.DisplayWakeUpJob
{
    /// <summary>
    /// Implements a Job, that turn on an Display/Monitor which is connected to a PowerAdapter.
    /// </summary>
    [Export("DisplayWakeUpJob", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    class DisplayWakeUp : IJob
    {
        #region Constants

        private const int AutoSleepTimerThreadCheckInterval = 5 * 1000;

        #endregion

        #region Fields

        private ISettingsServiceInternal _settings;

        private List<DisplayConfiguration> _configurations;

        private long _autoSleepAfterMinutes;
        private Thread _autoSleepTimerThread;
        private DateTime _lastAlarmTimestamp;

        private bool _ignoreErrorInResponse;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DisplayWakeUp class.
        /// </summary>
        public DisplayWakeUp()
        {
            _configurations = new List<DisplayConfiguration>();
        }

        #endregion

        #region Methods

        private void AutoSleepTimerThread()
        {
            while (true)
            {
                // Turn all displays off
                foreach (DisplayConfiguration display in _configurations)
                {
                    if (!display.IsTurnedOn)
                    {
                        continue;
                    }

                    // If the display was turned on, but now there is a new alarm,
                    // then update the timestamp so we don't accidentally turn it off again
                    if (display.TurnOnTimestamp < _lastAlarmTimestamp)
                    {
                        display.TurnOnTimestamp = _lastAlarmTimestamp;
                        continue;
                    }

                    long diffInMinutes = (long)(DateTime.UtcNow - display.TurnOnTimestamp).TotalMinutes;
                    if (diffInMinutes >= _autoSleepAfterMinutes)
                    {
                        Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.PutDisplayIntoSleepMessage, display.TurnOffMonitorUri.Authority);
                        display.TurnOff(_ignoreErrorInResponse);
                    }
                }

                Thread.Sleep(AutoSleepTimerThreadCheckInterval);
            }
        }

        #endregion

        #region IJob Members

        bool IJob.Initialize(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();

            string settingString = _settings.GetSetting("DisplayWakeUpJob", "DisplayConfiguration").GetValue<string>();
            _configurations.AddRange(DisplayConfiguration.ParseSettingString(settingString));

            if (_configurations.Count == 0)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.NoConfigurationsFoundError);
                return false;
            }

            _autoSleepAfterMinutes = _settings.GetSetting("DisplayWakeUpJob", "TurnOffTimeout").GetValue<int>();
            if (_autoSleepAfterMinutes > 0)
            {
                _autoSleepTimerThread = new Thread(AutoSleepTimerThread);
                _autoSleepTimerThread.Name = "Auto-sleep timer thread";
                _autoSleepTimerThread.Priority = ThreadPriority.BelowNormal;
                _autoSleepTimerThread.IsBackground = true;
                _autoSleepTimerThread.Start();
            }

            _ignoreErrorInResponse = _settings.GetSetting("DisplayWakeUpJob", "IgnoreErrorInResponse").GetValue<bool>();

            return true;
        }

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase != JobPhase.AfterOperationStored)
            {
                return;
            }

            if (_configurations.Count == 0)
            {
                return;
            }

            // Remember timestamp for automatically turning them off
            _lastAlarmTimestamp = DateTime.UtcNow;

            foreach (DisplayConfiguration dc in _configurations)
            {
                dc.TurnOn(_ignoreErrorInResponse);
            }
        }

        bool IJob.IsAsync
        {
            // This job is sync, because the asynchronity is done in the wrapper class
            get { return false; }
        }

        #endregion

        #region IDisposable Members

        void System.IDisposable.Dispose()
        {

        }

        #endregion

        #region Nested types

        class DisplayConfiguration
        {
            #region Constants

            private const int WebRequestTimeout = 5000;

            #endregion

            internal Uri TurnOnMonitorUri { get; private set; }
            internal Uri TurnOffMonitorUri { get; private set; }

            internal bool IsTurnedOn { get; set; }
            internal DateTime TurnOnTimestamp { get; set; }

            internal void TurnOn(bool ignoreErrors)
            {
                Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.BeginTurnOnDisplayMessage, TurnOnMonitorUri.Authority);
                SendRequestAsync(true, ignoreErrors);
            }

            internal void TurnOff(bool ignoreErrors)
            {
                Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.BeginTurnOffDisplayMessage, TurnOffMonitorUri.Authority);
                SendRequestAsync(false, ignoreErrors);
            }

            private void SendRequestAsync(bool state, bool ignoreErrors)
            {
                // Send the request asynchronously, we don't want to block the main thread!
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    Uri uri = state ? TurnOnMonitorUri : TurnOffMonitorUri;

                    bool success = false;

                    try
                    {
                        WebRequest request = WebRequest.Create(uri);
                        request.Timeout = WebRequestTimeout;
                        request.GetResponse();

                        // TODO: Examining the response may be helpful.

                        success = true;
                    }
                    catch (Exception)
                    {
                        if (ignoreErrors)
                        {
                            Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.ConnectionToDeviceFailedWithSuppression, uri);
                        }
                        else
                        {
                            Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.ConnectionToDeviceFailed, uri);
                        }
                    }
                    finally
                    {
                        if (success || (!success && ignoreErrors))
                        {
                            if (state)
                            {
                                TurnOnTimestamp = DateTime.UtcNow;
                            }
                            IsTurnedOn = state;
                        }
                    }
                });
            }

            internal static IEnumerable<DisplayConfiguration> ParseSettingString(string value)
            {
                using (StringReader reader = new StringReader(value))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Trim().Length == 0
                            || line.StartsWith("-"))
                        {
                            continue;
                        }

                        string[] tokens = line.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length != 2)
                        {
                            // TODO: Log warning
                            continue;
                        }

                        DisplayConfiguration dc = new DisplayConfiguration();
                        try
                        {
                            dc.TurnOnMonitorUri = new Uri(tokens[0]);
                            dc.TurnOffMonitorUri = new Uri(tokens[1]);
                        }
                        catch (UriFormatException)
                        {
                            Logger.Instance.LogFormat(LogType.Warning, null, Properties.Resources.ParseConfigFileUriFormatError, line);
                            continue;
                        }

                        yield return dc;
                    }
                }
            }

        }

        #endregion
    }
}