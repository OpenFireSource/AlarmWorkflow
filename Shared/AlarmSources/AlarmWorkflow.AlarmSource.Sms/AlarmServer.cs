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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.AlarmSource.Sms
{
    class AlarmServer
    {
        #region Fields

        private readonly SmsAlarmSource _parent;

        #endregion

        #region Properties

        internal TcpListener ServerSocket { get; set; }
        internal TcpClient ClientSocket { get; set; }

        #endregion

        #region Constructors

        internal AlarmServer(SmsAlarmSource parent)
        {
            //Sets the parent
            this._parent = parent;
            //Starts a Server listening on the Port 5555 and any IP Address
            ServerSocket = new TcpListener(IPAddress.Any, 5555);
            ClientSocket = default(TcpClient);
        }

        #endregion

        #region Methods

        internal void Start()
        {
            ServerSocket.Start();
            while (true)
            {
                ClientSocket = ServerSocket.AcceptTcpClient();
                HandleAlarmClient client = new HandleAlarmClient();
                client.StartClient(ClientSocket, _parent);
            }
        }

        internal void Stop()
        {
            if (ClientSocket != null)
            {
                ClientSocket.Close();
            }
            ServerSocket.Stop();
        }

        #endregion

    }


    class HandleAlarmClient
    {
        #region Fields

        private readonly UTF8Encoding _encoding = new UTF8Encoding();
        private TcpClient _clientSocket;
        private SmsAlarmSource _parent;

        #endregion

        #region Methods

        internal void StartClient(TcpClient inClientSocket, SmsAlarmSource parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            _clientSocket = inClientSocket;
            _parent = parent;

            Thread ctThread = new Thread(ReceiveThread);
            ctThread.Start();
        }

        private void ReceiveThread()
        {
            byte[] bytesFrom = new byte[10025];
            try
            {
                using (NetworkStream networkStream = _clientSocket.GetStream())
                {
                    int received = networkStream.Read(bytesFrom, 0, _clientSocket.ReceiveBufferSize);
                    if (received == 0)
                    {
                        return;
                    }

                    string alarmText = _encoding.GetString(bytesFrom, 0, received);
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