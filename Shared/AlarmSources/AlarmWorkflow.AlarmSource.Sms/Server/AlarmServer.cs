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

using System.Net;
using System.Net.Sockets;

namespace AlarmWorkflow.AlarmSource.Sms.Server
{
    class AlarmServer
    {
        #region Constants

        private const int ListenPort = 5555;

        #endregion

        #region Fields

        private readonly SmsAlarmSource _parent;
        private readonly TcpListener _serverSocket;
        private TcpClient _clientSocket;

        #endregion

        #region Constructors

        internal AlarmServer(SmsAlarmSource parent)
        {
            _parent = parent;

            _serverSocket = new TcpListener(IPAddress.Any, ListenPort);
        }

        #endregion

        #region Methods

        internal void Start()
        {
            _serverSocket.Start();

            while (true)
            {
                _clientSocket = _serverSocket.AcceptTcpClient();

                HandleAlarmClient client = new HandleAlarmClient();
                client.StartClient(_clientSocket, _parent);
            }
        }

        internal void Stop()
        {
            if (_clientSocket != null)
            {
                _clientSocket.Close();
            }

            _serverSocket.Stop();
        }

        #endregion
    }
}