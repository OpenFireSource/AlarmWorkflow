using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.WakeOnLan
{
    /// <summary>
    /// Implements a Job, that sends WakeUp-Packages.
    /// </summary>
    [Export("WakeOnLanJob", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    public class WakeOnLan : IJob
    {
        #region Fields

        private ISettingsServiceInternal _settings;

        private List<MacAddress> _macAddresses;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the WakeOnLanJob class.
        /// </summary>
        public WakeOnLan()
        {
            _macAddresses = new List<MacAddress>();
        }

        #endregion

        #region IJob Members

        public void Execute(IJobContext context, Operation operation)
        {
            if (context.Phase != JobPhase.AfterOperationStored)
            {
                return;
            }
            if (_macAddresses.Count == 0)
            {
                return;
            }

            _macAddresses.ForEach(x => x.WakeUp());
        }

        public bool Initialize(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();

            var settingString = _settings.GetSetting("WakeOnLanJob", "MacAddressConfiguration").GetValue<string>();
            settingString += "";
            _macAddresses.AddRange(MacAddress.ParseSettingString(settingString));

            if (_macAddresses.Count == 0)
            {
                Logger.Instance.LogFormat(LogType.Error, this, "There weren't found any Mac-Address");
                return false;
            }
            return true;
        }

        public bool IsAsync { get { return false; } }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {

        }

        #endregion


        #region Nested types

        class MacAddress
        {
            private byte[] _macBytes;

            public MacAddress(byte[] macBytes)
            {
                _macBytes = macBytes;
            }

            internal void WakeUp()
            {
                Logger.Instance.LogFormat(LogType.Trace, this, this.ToString());
                SendRequestAsync();
            }

            private void SendRequestAsync()
            {
                new Thread(() =>
                {
                    UdpClient client = new UdpClient();
                    client.Connect(IPAddress.Broadcast, 40000);

                    byte[] packet = new byte[17 * 6];

                    for (int i = 0; i < 6; i++)
                        packet[i] = 0xFF;

                    for (int i = 1; i <= 16; i++)
                        for (int j = 0; j < 6; j++)
                            packet[i * 6 + j] = _macBytes[j];

                    client.Send(packet, packet.Length);
                }).Start();
            }

            public override string ToString()
            {
                if (_macBytes == null || _macBytes.Length == 0)
                    return String.Empty;
                return _macBytes.Aggregate(_macBytes[0].ToString(), (current, macByte) => current + (":" + macByte));
            }

            internal static IEnumerable<MacAddress> ParseSettingString(string value)
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

                        string[] tokens = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length != 6)
                        {
                            // TODO: Warning Length
                            continue;
                        }


                        MacAddress mac;
                        try
                        {
                            byte[] macGroups = new byte[6];
                            for (int i = 0; i < 6; i++)
                            {
                                macGroups[i] = Byte.Parse(tokens[i]);
                            }
                            mac = new MacAddress(macGroups);
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.LogException(null, ex);
                            continue;
                        }

                        yield return mac;
                    }
                }
            }
        }

        #endregion
    }
}
