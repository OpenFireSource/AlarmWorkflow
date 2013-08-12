// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

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