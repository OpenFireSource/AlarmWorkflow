using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Job.DisplayWakeUpJob
{
    /// <summary>
    /// Implements a Job, that turn on an Display/Monitor which is connected to a PowerAdapter.
    /// </summary>
    [Export("DisplayWakeUpJob", typeof(IJob))]
    class DisplayWakeUp : IJob
    {
        #region Constants

        private const int AutoSleepTimerThreadCheckInterval = 30 * 1000;

        #endregion

        #region Fields

        private List<DisplayConfiguration> _configurations;

        private long _autoSleepAfterMinutes;
        private Thread _autoSleepTimerThread;
        private DateTime _lastAlarmTimestamp;

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
                        display.TurnOff();
                    }
                }

                Thread.Sleep(AutoSleepTimerThreadCheckInterval);
            }
        }

        #endregion

        #region IJob Members

        bool IJob.Initialize()
        {
            string settingString = SettingsManager.Instance.GetSetting("DisplayWakeUpJob", "DisplayConfiguration").GetString();
            _configurations.AddRange(DisplayConfiguration.ParseSettingString(settingString));

            if (_configurations.Count == 0)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.NoConfigurationsFoundError);
                return false;
            }

            _autoSleepAfterMinutes = SettingsManager.Instance.GetSetting("DisplayWakeUpJob", "TurnOffTimeout").GetInt32();
            if (_autoSleepAfterMinutes > 0)
            {
                _autoSleepTimerThread = new Thread(AutoSleepTimerThread);
                _autoSleepTimerThread.Priority = ThreadPriority.BelowNormal;
                _autoSleepTimerThread.Start();
            }

            return true;
        }

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (_configurations.Count == 0)
            {
                return;
            }

            // Remember timestamp for automatically turning them off
            _lastAlarmTimestamp = DateTime.UtcNow;

            foreach (DisplayConfiguration dc in _configurations)
            {
                dc.TurnOn();
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

            internal void TurnOn()
            {
                Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.BeginTurnOnDisplayMessage, TurnOnMonitorUri.Authority);
                SendRequestAsync(true);
            }

            internal void TurnOff()
            {
                Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.BeginTurnOffDisplayMessage, TurnOffMonitorUri.Authority);
                SendRequestAsync(false);
            }

            private void SendRequestAsync(bool state)
            {
                // Send the request asynchronously, we don't want to block the main thread!
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    Uri uri = state ? TurnOnMonitorUri : TurnOffMonitorUri;
                    try
                    {
                        WebRequest request = WebRequest.Create(uri);
                        request.Timeout = WebRequestTimeout;
                        request.GetResponse();

                        // When we get here we assume it was successful and set the properties appropriately
                        // TODO: Examining the response may be helpful.
                        if (state)
                        {
                            TurnOnTimestamp = DateTime.UtcNow;
                        }
                        IsTurnedOn = state;
                    }
                    catch (Exception)
                    {
                        Logger.Instance.LogFormat(LogType.Error, this, "Could not connect to the display for Uri '{0}'!", uri.ToString());
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
