﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Windows.UI.Models
{
    /// <summary>
    /// Represents the configuration for the Windows UI.
    /// </summary>
    internal sealed class UIConfiguration
    {
        #region Fields

        private double _scaleFactor;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the alias of the operation viewer to use. Empty means that the default viewer shall be used.
        /// </summary>
        public string OperationViewer { get; set; }
        /// <summary>
        /// Gets/sets the scale factor of the UI.
        /// </summary>
        public double ScaleFactor
        {
            get { return _scaleFactor; }
            set { _scaleFactor = Helper.Limit(0.5f, 4.0f, value); }
        }
        /// <summary>
        /// Gets/sets the automatic operation acknowledgement settings.
        /// </summary>
        public AutomaticOperationAcknowledgementSettings AutomaticOperationAcknowledgement { get; set; }
        /// <summary>
        /// Gets/sets the operation fetching parameters used by the worker thread.
        /// </summary>
        public OperationFetchingParameters OperationFetchingArguments { get; set; }
        /// <summary>
        /// Gets the names of the jobs that are enabled.
        /// </summary>
        public ReadOnlyCollection<string> EnabledJobs { get; private set; }
        /// <summary>
        /// Gets the key to press to acknowledge operations.
        /// </summary>
        public Key AcknowledgeOperationKey { get; private set; }
        /// <summary>
        /// Gets whether or not old operations shall be treated as new ones. Intended for debugging only!
        /// </summary>
        public bool TreatOldOperationsAsNew { get; private set; }
        /// <summary>
        /// Gets whether or not the selected alarm should change automatic, if mulitple alarms are available.
        /// </summary>
        public bool SwitchAlarms { get; private set; }
        /// <summary>
        /// Gets the time which should elapse between a change.
        /// </summary>
        public int SwitchTime { get; private set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UIConfiguration"/> class.
        /// </summary>
        public UIConfiguration()
        {
            AutomaticOperationAcknowledgement = new AutomaticOperationAcknowledgementSettings();
            OperationFetchingArguments = new OperationFetchingParameters();

            ScaleFactor = 2.0d;
            AcknowledgeOperationKey = System.Windows.Input.Key.B;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the UIConfiguration from its default path.
        /// </summary>
        /// <returns></returns>
        public static UIConfiguration Load()
        {
            UIConfiguration configuration = new UIConfiguration();
            configuration.OperationViewer = SettingsManager.Instance.GetSetting("UIConfiguration", "OperationViewer").GetString();
            configuration.ScaleFactor = SettingsManager.Instance.GetSetting("UIConfiguration", "ScaleFactor").GetValue<Double>();

            string acknowledgeOperationKeyS = SettingsManager.Instance.GetSetting("UIConfiguration", "AcknowledgeOperationKey").GetString();
            Key acknowledgeOperationKey = Key.B;
            Enum.TryParse<Key>(acknowledgeOperationKeyS, out acknowledgeOperationKey);
            configuration.AcknowledgeOperationKey = acknowledgeOperationKey;

            configuration.AutomaticOperationAcknowledgement.IsEnabled = SettingsManager.Instance.GetSetting("UIConfiguration", "AOA.IsEnabled").GetBoolean();
            configuration.AutomaticOperationAcknowledgement.MaxAge = SettingsManager.Instance.GetSetting("UIConfiguration", "AOA.MaxAge").GetInt32();

            configuration.OperationFetchingArguments.Interval = SettingsManager.Instance.GetSetting("UIConfiguration", "OFP.Interval").GetInt32();
            configuration.OperationFetchingArguments.MaxAge = SettingsManager.Instance.GetSetting("UIConfiguration", "OFP.MaxAge").GetInt32();
            configuration.OperationFetchingArguments.OnlyNonAcknowledged = SettingsManager.Instance.GetSetting("UIConfiguration", "OFP.OnlyNonAcknowledged").GetBoolean();
            configuration.OperationFetchingArguments.LimitAmount = SettingsManager.Instance.GetSetting("UIConfiguration", "OFP.LimitAmount").GetInt32();

            configuration.TreatOldOperationsAsNew = SettingsManager.Instance.GetSetting("UIConfiguration", "TreatOldOperationsAsNew").GetBoolean();

            // Jobs configuration
            configuration.EnabledJobs = new ReadOnlyCollection<string>(SettingsManager.Instance.GetSetting("UIConfiguration", "JobsConfiguration").GetValue<ExportConfiguration>().GetEnabledExports());

            configuration.SwitchAlarms = SettingsManager.Instance.GetSetting("UIConfiguration", "SwitchAlarms").GetBoolean();
            configuration.SwitchTime = SettingsManager.Instance.GetSetting("UIConfiguration", "SwitchTime").GetInt32();
            return configuration;
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Configures the automatic operation acknowledgement.
        /// </summary>
        public class AutomaticOperationAcknowledgementSettings
        {
            /// <summary>
            /// Gets/sets whether or not the automatic operation acknowledgement is enabled.
            /// </summary>
            public bool IsEnabled { get; set; }
            /// <summary>
            /// Gets/sets the maximum age in minutes until an operation is automatically acknowleded.
            /// </summary>
            public int MaxAge { get; set; }
        }

        /// <summary>
        /// Describes how operations shall be fetched by the worker thread.
        /// </summary>
        public class OperationFetchingParameters
        {
            /// <summary>
            /// Gets/sets the interval in milliseconds when to check for new operations. Sane values range between 1500-3000.
            /// </summary>
            public int Interval { get; set; }
            /// <summary>
            /// Gets/sets the maximum age of operations to get. Used to limit traffic and keep the interface clean. Set to '0' for no maximum age (not recommended).
            /// </summary>
            public int MaxAge { get; set; }
            /// <summary>
            /// Gets/sets whether or not to limit operation retrieval to non-acknowledged operations. An operation is acknowledged after it is "done".
            /// </summary>
            public bool OnlyNonAcknowledged { get; set; }
            /// <summary>
            /// Gets/sets the limit of the overall amount of operations. Used to limit traffic and keep the interface clean.Set to '0' for no limit (not recommended).
            /// </summary>
            public int LimitAmount { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="OperationFetchingParameters"/> class.
            /// </summary>
            public OperationFetchingParameters()
            {
                Interval = 2000;
                MaxAge = 7;
                OnlyNonAcknowledged = true;
                LimitAmount = 5;
            }
        }

        #endregion
    }
}
