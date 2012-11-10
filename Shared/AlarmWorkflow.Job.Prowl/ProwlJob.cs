//**********************//
//Philipp von Kirschbaum//
//**********************//

using System;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;
using Prowl;

namespace AlarmWorkflow.Job.Prowl
{
    [Export("ProwlJob", typeof(IJob))]
    class ProwlJob : IJob
    {
        #region private members

        /// <summary>
        /// The Prowl Configuration
        /// </summary>
        private ProwlClientConfiguration pcconfig;

        /// <summary>
        /// The Prowl Client
        /// </summary>
        private ProwlClient pclient;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ProwlJob class.
        /// </summary>
        public ProwlJob()
        {
        }

        #endregion

        #region IJob Members

        bool IJob.Initialize()
        {
            try
            {
                pcconfig = new ProwlClientConfiguration();

                pcconfig.ApplicationName = SettingsManager.Instance.GetSetting("ProwlJob", "ApplicationName").GetString();
                pcconfig.ProviderKey = SettingsManager.Instance.GetSetting("ProwlJob", "ProviderKey").GetString();
                pcconfig.ApiKeychain = SettingsManager.Instance.GetSetting("ProwlJob", "API").GetString();

                pclient = new ProwlClient(pcconfig);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void IJob.DoJob(Operation operation)
        {
            // Construct Notification text
            string body = "Einsatz:\r\n";
            body += "Zeitstempel: " + operation.Timestamp.ToString() + "\r\n";
            body += "Stichwort: " + operation.Keyword + "\r\n";
            body += "Einsatznr: " + operation.OperationNumber + "\r\n";
            body += "Hinweis: " + operation.Comment + "\r\n";
            body += "Mitteiler: " + operation.Messenger + "\r\n";
            body += "Einsatzort: " + operation.Location + "\r\n";
            body += "Straße: " + operation.Street + " " + operation.StreetNumber + "\r\n";
            body += "Ort: " + operation.ZipCode + " " + operation.City + "\r\n";
            body += "Objekt: " + operation.Property + "\r\n";

            ProwlNotification notifi = new ProwlNotification();
            notifi.Priority = ProwlNotificationPriority.Emergency;
            notifi.Event = "Feuerwehr Einsatz";
            notifi.Description = body;

            //Send the Message
            try
            {
                pclient.PostNotification(notifi);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, "An error occurred while sending the Prowl Messages.");
                Logger.Instance.LogException(this, ex);
            }
        }

        #endregion
    }
}
