using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

// See https://www.notifymyandroid.com/api.jsp#notify for NMA-API information.

namespace AlarmWorkflow.Job.GrowlJob
{
    /// <summary>
    /// Provides an implementation of the <see cref="IGrowlSender"/>-interface to send Growl notifications via the "Notify my Android" (NMA) web site.
    /// </summary>
    [Export("NotifyMyAndroidGrowlSender", typeof(IGrowlSender))]
    sealed class NotifyMyAndroidGrowlSender : IGrowlSender
    {
        #region Constants

        private const string EmergencyPriority = "2";

        #endregion

        #region IGrowlSender Members

        void IGrowlSender.SendNotification(IGrowlJob growl, string headline, string text)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("https://www.notifymyandroid.com/publicapi/notify?");
            // API-Key(s)
            sb.AppendFormat("apikey={0}", string.Join(",", growl.GetApiKeys()));
            // Application name
            sb.AppendFormat("&application={0}", HttpUtility.UrlEncode(growl.ApplicationName));
            // Event (max. 1000 chars)
            sb.AppendFormat("&event={0}", HttpUtility.UrlEncode(headline.Truncate(1000, true, true)));
            // Description (max. 10000 chars)
            sb.AppendFormat("&description={0}", HttpUtility.UrlEncode(text.Truncate(10000, true, true)));
            // Priority
            sb.AppendFormat("&priority={0}", EmergencyPriority);

            WebRequest notificationRequest = WebRequest.Create(sb.ToString());
            WebResponse notificationResponse = notificationRequest.GetResponse();

            NMAResponse logicalResponse = NMAResponse.ReadResponse(notificationResponse);
            if (!logicalResponse.IsSuccess)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.NMAApiKeyIsInvalid, logicalResponse.Message);
            }
            if (!logicalResponse.CanSendAnotherRequest)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.NMANoRemainingRequestsLeft, logicalResponse.ResetTimer);
            }
        }

        #endregion

        #region Nested types

        class NMAResponse
        {
            internal string Text;
            internal int Code;
            internal string Message;

            internal int RemainingRequests;
            internal int ResetTimer;

            internal bool IsSuccess
            {
                get { return Text == "success" && Code == 200; }
            }

            /// <summary>
            /// Returns whether or not at least one other request can be sent until we have no more "Remaining Requests" left.
            /// </summary>
            internal bool CanSendAnotherRequest
            {
                get { return IsSuccess && RemainingRequests > 0; }
            }

            /// <summary>
            /// Reads the WebResponse and parses the information into a new friendly object.
            /// </summary>
            /// <param name="response"></param>
            /// <returns></returns>
            internal static NMAResponse ReadResponse(WebResponse response)
            {
                NMAResponse lr = new NMAResponse();

                // Read resulting XML
                XDocument doc = XDocument.Load(response.GetResponseStream());

                XElement resultNode = doc.Root.Elements().First();
                // The first element underneath the Root is called different, but it always has an attribute "code"
                lr.Text = resultNode.Name.LocalName;
                lr.Code = int.Parse(resultNode.Attribute("code").Value);

                // Check return message
                if (lr.IsSuccess)
                {
                    // Even if it was successful, we have a limited amount of requests.
                    lr.RemainingRequests = int.Parse(resultNode.Attribute("remaining").Value);
                    lr.ResetTimer = int.Parse(resultNode.Attribute("resettimer").Value);
                }
                else
                {
                    // If we have a wrong API Key (or any other error occurred) write it to the log file and return false.
                    lr.Message = resultNode.Value;
                }

                return lr;
            }
        }

        #endregion
    }
}
