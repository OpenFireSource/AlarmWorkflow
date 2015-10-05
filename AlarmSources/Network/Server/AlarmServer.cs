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

using AlarmWorkflow.Shared.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AlarmWorkflow.AlarmSource.Network.Server
{
    class AlarmServer
    {

        #region Properties
        
        public bool IsRunning { get; private set; }

        #endregion

        #region Fields

        private readonly NetworkAlarmSource _parent;
        private TcpListener _serverSocket;
        private TcpClient _clientSocket;
        private int _listenPort;
		
        #endregion

        #region Constructors

        internal AlarmServer(NetworkAlarmSource parent, int listenPort)
        {
            _parent = parent;
            _listenPort = listenPort;
            _serverSocket = new TcpListener(IPAddress.Any, _listenPort);
        }

        #endregion

        #region Methods

        internal void Start()
        {
            try
            {
                _serverSocket.Start();
                IsRunning = true;

                Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.NetworkServerStarted, _listenPort);
            }
            catch (SocketException ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.NetworkSocketError, ex);
                Logger.Instance.LogException(this, ex);
            }

            while (true)
            {
                try
                {
                    _clientSocket = _serverSocket.AcceptTcpClient();
                    HandleAlarmClient.StartClient(_clientSocket, _parent);
                }
                catch(Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.NetworkError, ex);
                    Logger.Instance.LogException(this, ex);
                }
            }
        }

        internal void Stop()
        {
            if (_clientSocket != null)
            {
                _clientSocket.Close();
            }

            _serverSocket.Stop();
            IsRunning = false;
        }

        #endregion
    }
}
