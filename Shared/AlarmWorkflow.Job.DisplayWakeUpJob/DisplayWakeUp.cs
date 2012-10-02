using System;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.DisplayWakeUpJob
{
    /// <summary>
    /// Implements a Job, that turn on an Display/Monitor which is connected to a PowerAdapter.
    /// </summary>
    public class DisplayWakeUp : IJob
    {
        #region private members
        /// <summary>
        /// The errormsg, if an error occured.
        /// </summary>
        private string errormsg;

        /// <summary>
        /// The IP of the PowerAdapter.
        /// </summary>
        private string ip = string.Empty;

        /// <summary>
        /// The password if some is needed.
        /// </summary>
        private string pwd = string.Empty;

        /// <summary>
        /// The user name if some is needed.
        /// </summary>
        private string user = string.Empty;

        /// <summary>
        /// The port for the PowerAdapter.
        /// </summary>
        private string port = string.Empty;
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

        string IJob.ErrorMessage
        {
            get
            {
                return this.errormsg;
            }
        }

        void IJob.Initialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Config\DisplayWakeUpJobConfiguration.xml");

            IXPathNavigable settings = doc.CreateNavigator().SelectSingleNode("Display");

            XPathNavigator nav = settings.CreateNavigator();
            if (nav.UnderlyingObject is XmlElement)
            {
                this.ip = ((XmlElement)nav.UnderlyingObject).Attributes["ip"].InnerText;
                this.user = ((XmlElement)nav.UnderlyingObject).Attributes["user"].InnerText;
                this.pwd = ((XmlElement)nav.UnderlyingObject).Attributes["pwd"].InnerText;
                this.port = ((XmlElement)nav.UnderlyingObject).Attributes["port"].InnerText;
            }
            else
            {
                throw new ArgumentException("Settings is not an XmlElement");
            }
        }

        bool IJob.DoJob(Operation einsatz)
        {
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
            try
            {
                HttpWebRequest msg = (HttpWebRequest)System.Net.WebRequest.Create(new Uri(builder.ToString()));
                msg.GetResponse();
            }
            catch (ArgumentNullException ex)
            {
                this.errormsg = ex.ToString();
                return false;
            }
            catch (WebException ex)
            {
                this.errormsg = ex.ToString();
                return false;
            }
            catch (NotSupportedException ex)
            {
                this.errormsg = ex.ToString();
                return false;
            }
            catch (ProtocolViolationException ex)
            {
                this.errormsg = ex.ToString();
                return false;
            }
            catch (InvalidOperationException ex)
            {
                this.errormsg = ex.ToString();
                return false;
            }
            catch (SecurityException ex)
            {
                this.errormsg = ex.ToString();
                return false;
            }

            return true;
        }

        #endregion
    }
}
