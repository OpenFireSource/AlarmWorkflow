using System;
using System.Collections.Generic;
using System.Text;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Job.GrowlJob
{
    [Export("GrowlJob", typeof(IJob))]
    sealed class GrowlJob : IJob, IGrowlJob
    {
        #region Constants

        private const string ApplicationName = "AlarmWorkflow";

        #endregion

        #region Fields

        private List<IGrowlSender> _growlSender;

        private Lazy<string[]> _apiKeys;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GrowlJob"/> class.
        /// </summary>
        public GrowlJob()
        {
            _growlSender = new List<IGrowlSender>();

            _apiKeys = new Lazy<string[]>(() => SettingsManager.Instance.GetSetting("GrowlJob", "APIKeys").GetString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
        }

        #endregion

        #region IJob Members

        void IJob.DoJob(Operation operation)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Einsatznummer: {0}", operation.OperationNumber).AppendLine();
            sb.AppendFormat("Einsatzort: {0}", operation.GetDestinationLocation());

            // Send notification via each growl sender
            _growlSender.ForEach(gs =>
            {
                try
                {
                    gs.SendNotification(this, "Neuer Alarm!", sb.ToString());
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, "Could not send Growl notification! See log for information.");
                    Logger.Instance.LogException(this, ex);
                }
            });
        }

        bool IJob.Initialize()
        {
            // Get export configuration
            ExportConfiguration exports = SettingsManager.Instance.GetSetting("GrowlJob", "GrowlSender").GetValue<ExportConfiguration>();

            // Add each export to the list, if initialization succeeded
            foreach (string exportAlias in exports.GetEnabledExports())
            {
                try
                {
                    IGrowlSender growlSender = ExportedTypeLibrary.Import<IGrowlSender>(exportAlias);

                    if (!growlSender.IsConfigured(this))
                    {
                        // This plug-in is not properly configured.
                        Logger.Instance.LogFormat(LogType.Info, this, "Skipping growl sender '{0}' because it reported not being properly configured. See log for information.", exportAlias);
                        continue;
                    }

                    // This one has passed!
                    _growlSender.Add(growlSender);
                    Logger.Instance.LogFormat(LogType.Info, this, "Added growl sender '{0}'.", exportAlias);
                }
                catch
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, "An error occurred while initialization plug-in '{0}'. Ignoring plug-in.", exportAlias);
                }
            }

            return true;
        }

        #endregion

        #region IGrowlJob Members

        string IGrowlJob.ApplicationName
        {
            get { return ApplicationName; }
        }

        string[] IGrowlJob.GetApiKeys()
        {
            return _apiKeys.Value;
        }

        #endregion
    }
}
