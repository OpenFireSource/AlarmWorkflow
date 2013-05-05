using System;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Shared.Specialized.Printing
{
    /// <summary>
    /// Provides access to the configured printing queues.
    /// </summary>
    public static class PrintingQueueManager
    {
        private static readonly Lazy<PrintingQueuesConfiguration> _printingQueues;

        /// <summary>
        /// Initializes the <see cref="PrintingQueueManager"/> class.
        /// </summary>
        static PrintingQueueManager()
        {
            _printingQueues = new Lazy<PrintingQueuesConfiguration>(() => SettingsManager.Instance.GetSetting("Shared", "PrintingQueuesConfiguration").GetValue<PrintingQueuesConfiguration>(), true);
        }

        /// <summary>
        /// Returns the <see cref="PrintingQueuesConfiguration"/>-instance of the current AppDomain.
        /// The configuration is created on-demand and is then cached.
        /// </summary>
        /// <returns>The <see cref="PrintingQueuesConfiguration"/>-instance of the current AppDomain.</returns>
        public static PrintingQueuesConfiguration GetInstance()
        {
            return _printingQueues.Value;
        }
    }
}
