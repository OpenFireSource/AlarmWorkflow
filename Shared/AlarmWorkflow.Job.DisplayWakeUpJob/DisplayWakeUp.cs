using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.DisplayWakeUpJob
{
    /// <summary>
    /// Implements a Job, that turn on an Display/Monitor which is connected to a PowerAdapter.
    /// </summary>
    [Export("DisplayWakeUpJob", typeof(IJob))]
    class DisplayWakeUp : IJob
    {
        #region private members

        private string ip = string.Empty;
        private string pwd = string.Empty;
        private string user = string.Empty;
        private string port = string.Empty;

        private string _httpRequest;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the DisplayWakeUp class.
        /// </summary>
        public DisplayWakeUp()
        {
        }

        #endregion

        #region IJob Members

        bool IJob.Initialize()
        {
            XDocument doc = XDocument.Load(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Config\DisplayWakeUpJobConfiguration.xml");

            // TODO: Allow multiple screens!
            foreach (var item in doc.Root.Elements("Display"))
            {
                this.ip = item.Attribute("ip").Value;
                this.user = item.Attribute("user").Value;
                this.pwd = item.Attribute("pwd").Value;
                this.port = item.Attribute("port").Value;
            }

            StringBuilder builder = new StringBuilder();

            //// http://admin:TFTPowerControl@192.168.0.243:80/SWITCH.CGI?s1=1

            builder.Append("http://");
            if (string.IsNullOrEmpty(this.user) == false)
            {
                builder.Append(this.user);
                builder.Append(":");
                builder.Append(this.pwd);
                builder.Append("@");
            }

            builder.Append(this.ip);
            builder.Append(":");
            builder.Append(this.port);
            builder.Append("/SWITCH.CGI?s1=1");

            _httpRequest = builder.ToString();
            return true;
        }

        void IJob.DoJob(Operation einsatz)
        {
            if (_httpRequest == null)
            {
                return;
            }

            try
            {
                HttpWebRequest msg = (HttpWebRequest)System.Net.WebRequest.Create(new Uri(_httpRequest));
                msg.GetResponse();
            }
            catch (Exception)
            {
                _httpRequest = null;
                Logger.Instance.LogFormat(LogType.Error, this, "Could not connect to the display. Disabling job!");
            }
        }

        #endregion
    }
}
