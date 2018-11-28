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

using AlarmWorkflow.Backend.Data;
using AlarmWorkflow.Backend.Data.Types;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Threading;

namespace AlarmWorkflow.BackendService.System.Data
{
    static class DatabaseChecker
    {
        #region Constants

        private const int CheckConnectionRetryTimeoutMs = 1000;

        #endregion

        #region Methods

        internal static void EnsureReachable(IServiceProvider serviceProvider)
        {
            Logger.Instance.LogFormat(LogType.Trace, typeof(DatabaseChecker), Properties.Resources.DatabasePresenceCheckBegin);

            while (!CheckDatabaseReachable(serviceProvider))
            {
                Logger.Instance.LogFormat(LogType.Warning, typeof(DatabaseChecker), Properties.Resources.DatabaseNotReachableErrorMessage, CheckConnectionRetryTimeoutMs);

                Thread.Sleep(CheckConnectionRetryTimeoutMs);
            }

            Logger.Instance.LogFormat(LogType.Trace, typeof(DatabaseChecker), Properties.Resources.DatabasePresenceCheckFinished);
        }

        private static bool CheckDatabaseReachable(IServiceProvider services)
        {
            try
            {
                /* Creating a new unit of work will automatically create a connection to the appropriate database.
                 * Then we only have to access some table, which opens the connection, and the query will result in either success or an error.
                 */
                using (IUnitOfWork work = services.GetService<IDataContextFactory>().Get().Create())
                {
                    // Migrate to new database versions
                    work.Database.Migrate();

                    work.For<SettingData>().Query.FirstOrDefault();
                }

                return true;
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1050)
                {
                    // Table already exists. 
                    // This happens when the user upgrade from the entity framework to entity framework core.
                    // The script tries to execute the migration, but the table already exist. So we have to add the migration by hand
                    // into the table. This is really dirty and should be changed ..
                    using (IUnitOfWork work = services.GetService<IDataContextFactory>().Get().Create())
                    {
                        work.Database.ExecuteSqlCommand(@"REPLACE INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES ('20181230152708_Initial', '2.2.0-rtm-35687');");
                    }
                }
                else
                {
                    Logger.Instance.LogException("DatabaseChecker", ex);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException("DatabaseChecker", ex);
            }

            return false;
        }

        #endregion
    }
}
