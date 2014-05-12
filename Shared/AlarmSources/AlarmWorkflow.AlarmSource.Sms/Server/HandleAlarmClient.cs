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

namespace AlarmWorkflow.AlarmSource.Sms.Server
{
    class HandleAlarmClient
    {
        #region Constants

        private const int BufferSize = 10025;

        #endregion

        #region Fields

        private readonly UTF8Encoding _encoding;
        private TcpClient _clientSocket;
        private SmsAlarmSource _parent;

        #endregion

        #region Constructors

        internal HandleAlarmClient()
        {
            _encoding = new UTF8Encoding();
        }

        #endregion

        #region Methods

        internal void StartClient(TcpClient clientSocket, SmsAlarmSource parent)
        {
            if (clientSocket == null)
            {
                throw new ArgumentNullException("clientSocket");
            }
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            _clientSocket = clientSocket;
            _parent = parent;

            Thread ctThread = new Thread(ReceiveThread);
            ctThread.Name = Properties.Resources.SmsAlarmClientThreadName;
            ctThread.Priority = ThreadPriority.BelowNormal;
            ctThread.IsBackground = true;
            ctThread.Start();
        }

        private void ReceiveThread()
        {
            byte[] buffer = new byte[BufferSize];
            try
            {
                using (NetworkStream networkStream = _clientSocket.GetStream())
                {
                    int received = networkStream.Read(buffer, 0, _clientSocket.ReceiveBufferSize);
                    if (received == 0)
                    {
                        return;
                    }

                    string alarmText = _encoding.GetString(buffer, 0, received);

                    _parent.PushIncomingAlarm(alarmText);
                }
            }

            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
            }
        }

        #endregion
    }
}