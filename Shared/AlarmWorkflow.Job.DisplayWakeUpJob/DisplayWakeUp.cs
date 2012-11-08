using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
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
        #region Fields

        private List<DisplayConfiguration> _configurations;

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

            int turnOffTimeout = SettingsManager.Instance.GetSetting("DisplayWakeUpJob", "TurnOffTimeout").GetInt32();
            if (turnOffTimeout > 0)
            {
                // TODO: Start background thread which checks all 60 seconds for automatically turning
            }

            return true;
        }

        void IJob.DoJob(Operation operation)
        {
            if (_configurations.Count == 0)
            {
                return;
            }

            foreach (DisplayConfiguration dc in _configurations)
            {
                dc.TurnOn();
            }
        }

        #endregion

        #region Nested types

        class DisplayConfiguration
        {
            private const int WebRequestTimeout = 5000;

            internal Uri TurnOnMonitorUri { get; private set; }
            internal Uri TurnOffMonitorUri { get; private set; }

            internal bool IsTurnedOn { get; set; }
            internal DateTime TurnOnTimestamp { get; set; }

            internal void TurnOn()
            {
                SendRequest(true);
                TurnOnTimestamp = DateTime.UtcNow;
            }

            internal void TurnOff()
            {
                SendRequest(false);
            }

            private void SendRequest(bool state)
            {
                // Send the request asynchronously, we don't want to block the main thread!
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    Uri uri = state ? TurnOnMonitorUri : TurnOffMonitorUri;
                    try
                    {

                        HttpWebRequest msg = (HttpWebRequest)System.Net.WebRequest.Create(uri);
                        msg.Timeout = WebRequestTimeout;
                        msg.GetResponse();
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
