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