using System.IO;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.PrintingUIJob.Config
{
    /// <summary>
    /// Represents the configuration of this job.
    /// </summary>
    internal sealed class Configuration
    {
        #region Properties

        /// <summary>
        /// Gets whether or not this job is enabled.
        /// </summary>
        public bool IsEnabled { get; private set; }
        /// <summary>
        /// Gets the name of the printer to use for printing. Blank uses the printer marked as default.
        /// </summary>
        public string PrinterName { get; private set; }
        /// <summary>
        /// Gets the amount of copies to print per job/operation.
        /// </summary>
        public int CopyCount { get; private set; }
        /// <summary>
        /// Gets the interval in milliseconds to wait until printing is done.
        /// This has the sole purpose to let the UI wait to update its bindings etc., which sometimes
        /// just doesn't happen if we immediately, for example, print directly after the bindings should have been updated.
        /// Don't set this interval to high!
        /// </summary>
        public int WaitInterval { get; private set; }
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
            string configFile = Path.Combine(Utilities.GetWorkingDirectory(), "Config\\PrintingUIJobConfiguration.xml");
            if (configFile == null)
            {
                return null;
            }

            Configuration configuration = new Configuration();

            XDocument doc = XDocument.Load(configFile);
            configuration.PrinterName = doc.Root.TryGetElementValue("PrinterName", null);
            configuration.CopyCount = doc.Root.TryGetElementValue("CopyCount", 1);
            configuration.WaitInterval = doc.Root.TryGetElementValue("WaitInterval", 50);
            configuration.RememberPrintedOperations = doc.Root.TryGetElementValue("RememberPrintedOperations", true);

            return configuration;
        }

        #endregion

    }
}
