/*
//
//
//
Mehr Informationen unter http://www.sipgate.de/basic
//
//
//
*/

using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using AlarmWorkflow.Shared.Diagnostics;
using sipgate.samurai;
using sipgate;

namespace AlarmWorkflow.Job.SmsJob.Providers
{
    [Export("SipGate", typeof(ISmsProvider))]
    class sipgate : ISmsProvider
    {
        #region ISmsProvider Members

        void ISmsProvider.Send(string userName, string password, IEnumerable<string> phoneNumbers, string messageText)
        {
            Session s = new Session(userName, password);
            foreach (string nummber in phoneNumbers)
            {
                try
                {
                    s.sendSms(nummber, messageText);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SendToRecipientGenericErrorMessage, nummber);
                    Logger.Instance.LogException(this, ex);
                }
            }
        }

        #endregion
    }
}