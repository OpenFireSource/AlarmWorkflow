using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AlarmWorkflow.AlarmSource.Sms
{
    public class Alarmserver
    {
        private readonly SmsAlarmSource parent;

        /// <summary>
        /// 
        /// </summary>
        public Alarmserver(SmsAlarmSource parent)
        {
            //Sets the parent
            this.parent = parent;
            //Starts a Server listening on the Port 5555 and any IP Address
            serverSocket = new TcpListener(IPAddress.Any, 5555);
            clientSocket = default(TcpClient);
        }

        internal void start()
        {
            serverSocket.Start();
            while (true)
            {
                clientSocket = serverSocket.AcceptTcpClient();
                handleAlarmClient client = new handleAlarmClient();
                client.startClient(clientSocket, parent);
            }
        }

        internal void stop()
        {
            clientSocket.Close();
            serverSocket.Stop();
        }

        public TcpListener serverSocket { get; set; }

        public TcpClient clientSocket { get; set; }
      
    }


    public class handleAlarmClient
    {

        TcpClient clientSocket;
        private SmsAlarmSource parent;
        public void startClient(TcpClient inClientSocket, SmsAlarmSource parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            clientSocket = inClientSocket;
            this.parent = parent;
            Thread ctThread = new Thread(recive);
            ctThread.Start();
        }

        private void recive()
        {
            byte[] bytesFrom = new byte[10025];
            try
            {
                NetworkStream networkStream = clientSocket.GetStream();
                int received = networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                if (received == 0)
                {
                    return;
                }
                var encoding = new UTF8Encoding();
                string empfangen = encoding.GetString(bytesFrom, 0, received);
                    parent.incommingAlarm(empfangen);
                
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
