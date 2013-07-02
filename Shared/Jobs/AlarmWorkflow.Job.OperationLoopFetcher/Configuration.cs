using System;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Job.OperationLoopFetcher
{
    class Configuration
    {
        #region Properties

        /// <summary>
        /// Gets the path to the file that contains the loop information written by the batch file.
        /// </summary>
        public string LoopsFilePath { get; private set; }
        /// <summary>
        /// Gets the maximum age of one entry to be considered for an alarm.
        /// </summary>
        public TimeSpan MaxEntryAge { get; private set; }
        /// <summary>
        /// Gets the format of the entry date/time.
        /// </summary>
        public string EntryDateTimeFormat { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class
        /// and loads the setting values immediately.
        /// </summary>
        public Configuration()
        {
            LoopsFilePath = SettingsManager.Instance.GetSetting("OperationLoopFetcherJob", "LoopsFilePath").GetString();
            MaxEntryAge = TimeSpan.FromSeconds(SettingsManager.Instance.GetSetting("OperationLoopFetcherJob", "MaxEntryAge").GetInt32());
            EntryDateTimeFormat = SettingsManager.Instance.GetSetting("OperationLoopFetcherJob", "EntryDateTimeFormat").GetString();
        }

        #endregion
    }
}
