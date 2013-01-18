using System.Collections.ObjectModel;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Shared
{
    /// <summary>
    /// Represents the current configuration.
    /// </summary>
    public sealed class AlarmWorkflowConfiguration
    {
        #region Fields

        private static AlarmWorkflowConfiguration _instance;
        /// <summary>
        /// Gets the current Configuration.
        /// </summary>
        public static AlarmWorkflowConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AlarmWorkflowConfiguration();
                }
                return _instance;
            }
        }

        #endregion

        #region Properties

        internal string OperationStoreAlias { get; private set; }
        internal string RoutePlanProviderAlias { get; private set; }
        internal bool DownloadRoutePlan { get; private set; }
        internal int RouteImageHeight { get; private set; }
        internal int RouteImageWidth { get; private set; }

        /// <summary>
        /// Gets the information about the current fire department site.
        /// </summary>
        /// <remarks>This information is used (among others) to provide the route information to the operation destination.</remarks>
        public FireDepartmentInfo FDInformation { get; private set; }

        internal ReadOnlyCollection<string> EnabledJobs { get; private set; }
        internal ReadOnlyCollection<string> EnabledAlarmSources { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="AlarmWorkflowConfiguration"/> class from being created.
        /// </summary>
        private AlarmWorkflowConfiguration()
        {
            this.OperationStoreAlias = SettingsManager.Instance.GetSetting("Shared", "OperationStore").GetString();
            this.RoutePlanProviderAlias = SettingsManager.Instance.GetSetting("Shared", "RoutePlanProvider").GetString();

            this.DownloadRoutePlan = SettingsManager.Instance.GetSetting("Shared", "DownloadRoutePlan").GetBoolean();
            this.RouteImageHeight = SettingsManager.Instance.GetSetting("Shared", "RouteImageHeight").GetInt32();
            this.RouteImageWidth = SettingsManager.Instance.GetSetting("Shared", "RouteImageWidth").GetInt32();

            // FD Information
            this.FDInformation = new FireDepartmentInfo();
            this.FDInformation.Name = SettingsManager.Instance.GetSetting("Shared", "FD.Name").GetString();
            this.FDInformation.Location = new PropertyLocation();
            this.FDInformation.Location.ZipCode = SettingsManager.Instance.GetSetting("Shared", "FD.ZipCode").GetString();
            this.FDInformation.Location.City = SettingsManager.Instance.GetSetting("Shared", "FD.City").GetString();
            this.FDInformation.Location.Street = SettingsManager.Instance.GetSetting("Shared", "FD.Street").GetString();
            this.FDInformation.Location.StreetNumber = SettingsManager.Instance.GetSetting("Shared", "FD.StreetNumber").GetString();

            // Jobs and AlarmSources configuration
            this.EnabledJobs = new ReadOnlyCollection<string>(SettingsManager.Instance.GetSetting("Shared", "JobsConfiguration").GetValue<ExportConfiguration>().GetEnabledExports());
            this.EnabledAlarmSources = new ReadOnlyCollection<string>(SettingsManager.Instance.GetSetting("Shared", "AlarmSourcesConfiguration").GetValue<ExportConfiguration>().GetEnabledExports());

            _instance = this;
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents information about the current fire department site.
        /// </summary>
        public class FireDepartmentInfo
        {
            /// <summary>
            /// Gets the name of the site.
            /// </summary>
            public string Name { get; internal set; }
            /// <summary>
            /// Gets the location of the site.
            /// </summary>
            public PropertyLocation Location { get; internal set; }
        }


        #endregion
    }
}
