using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Windows.PrintingUIJob
{
    /// <summary>
    /// Represents the configuration of this job.
    /// </summary>
    internal sealed class Configuration
    {
        #region Properties

        /// <summary>
        /// Gets the Uri of the print server. Leave blank to use the local print server.
        /// </summary>
        public string PrintServer { get; private set; }
        /// <summary>
        /// Gets the name of the printer to use for printing. Blank uses the printer marked as default.
        /// </summary>
        public string PrinterName { get; private set; }
        /// <summary>
        /// Gets the amount of copies to print per job/operation.
        /// </summary>
        public int CopyCount { get; private set; }
        /// <summary>
        /// Gets whether or not to avoid printing operations that have already been printed.
        /// </summary>
        public bool RememberPrintedOperations { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the Configuration from its default path.
        /// </summary>
        /// <returns></returns>
        public static Configuration Load()
        {
            Configuration configuration = new Configuration();
            configuration.PrintServer = SettingsManager.Instance.GetSetting("PrintingUIJob", "PrintServer").GetString();
            if (string.IsNullOrWhiteSpace(configuration.PrintServer))
            {
                configuration.PrintServer = null;
            }

            configuration.PrinterName = SettingsManager.Instance.GetSetting("PrintingUIJob", "PrinterName").GetString();
            if (string.IsNullOrWhiteSpace(configuration.PrinterName))
            {
                configuration.PrinterName = null;
            }

            configuration.CopyCount = SettingsManager.Instance.GetSetting("PrintingUIJob", "CopyCount").GetInt32();
            configuration.RememberPrintedOperations = SettingsManager.Instance.GetSetting("PrintingUIJob", "RememberPrintedOperations").GetBoolean();

            return configuration;
        }

        #endregion

    }
}
