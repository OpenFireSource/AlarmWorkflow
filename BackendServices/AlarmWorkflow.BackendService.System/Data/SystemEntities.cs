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
using System.Threading;
using AlarmWorkflow.Backend.ServiceContracts.Data;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.BackendService.System.Data
{
    partial class SystemEntities
    {
        #region Constants

        private const string EdmxPath = "Data.SystemEntities";
        private const int CheckConnectionRetryTimeoutMs = 1000;

        #endregion

        #region Methods

        /// <summary>
        /// Ensures that we have database access to the configured server.
        /// If database access is not possible, consecutive attempts are made after waiting a little.
        /// </summary>
        internal static void EnsureDatabaseReachable()
        {
            Logger.Instance.LogFormat(LogType.Trace, typeof(SystemEntities), Properties.Resources.DatabasePresenceCheckBegin);

            while (!CheckDatabaseReachable())
            {
                Logger.Instance.LogFormat(LogType.Warning, typeof(SystemEntities), Properties.Resources.DatabaseNotReachableErrorMessage, CheckConnectionRetryTimeoutMs);

                Thread.Sleep(CheckConnectionRetryTimeoutMs);
            }

            Logger.Instance.LogFormat(LogType.Trace, typeof(SystemEntities), Properties.Resources.DatabasePresenceCheckFinished);
        }

        private static bool CheckDatabaseReachable()
        {
            using (SystemEntities entities = EntityFrameworkHelper.CreateContext<SystemEntities>(EdmxPath))
            {
                try
                {
                    entities.Connection.Open();
                    return true;
                }
                catch (Exception)
                {
                    // Intentionally left blank --> database not reachable or other error.
                }
            }
            return false;
        }

        #endregion
    }
}
