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
