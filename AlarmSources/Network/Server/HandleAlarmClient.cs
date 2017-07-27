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
using System.Net.Sockets;
using System.Text;
using System.Threading;
using AlarmWorkflow.Shared.Diagnostics;
using System.IO;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.AlarmSource.Network.Server
{
    class HandleAlarmClient
    {
        #region Constants

        private const int BufferSize = 1024;

        #endregion

        #region Fields

        private readonly UTF8Encoding _encoding;
        private TcpClient _clientSocket;
        private NetworkAlarmSource _parent;

        #endregion

        #region Constructors

        private HandleAlarmClient()
        {
            _encoding = new UTF8Encoding();
        }

        #endregion

        #region Methods

        internal static void StartClient(TcpClient clientSocket, NetworkAlarmSource parent)
        {
            if (clientSocket == null)
            {
                throw new ArgumentNullException("clientSocket");
            }
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            HandleAlarmClient handleAlarmClient = new HandleAlarmClient
            {
                _clientSocket = clientSocket,
                _parent = parent
            };

            Thread ctThread = new Thread(handleAlarmClient.ReceiveThread)
            {
                Name = Properties.Resources.NetworkAlarmClientThreadName,
                Priority = ThreadPriority.BelowNormal,
                IsBackground = true
            };
            ctThread.Start();
        }

        private void ReceiveThread()
        {
            byte[] buffer = new byte[BufferSize];
            try
            {
                using (NetworkStream networkStream = _clientSocket.GetStream())
                using (MemoryStream ms = new MemoryStream())
                {
                    int numBytesRead;
                    while ((numBytesRead = networkStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, numBytesRead);
                    }

                    string alarmJson = _encoding.GetString(ms.ToArray(), 0, (int)ms.Length);
                    Operation operation = Json.Deserialize<Operation>(alarmJson);

                    _parent.PushIncomingAlarm(operation);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.NetworkParserError, ex);
                Logger.Instance.LogException(this, ex);
            }
        }

        #endregion

    }
}