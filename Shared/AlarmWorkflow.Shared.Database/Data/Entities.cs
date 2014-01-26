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

using System.Linq;
using System.Text;
using System.Threading;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Shared.Database.Data
{
    partial class AlarmWorkflowEntities
    {
        #region Constants

        private static readonly string ConnectionString;
        private const int CheckConnectionRetryTimeoutMs = 1000;

        #endregion

        #region Constructors

        static AlarmWorkflowEntities()
        {
            ConnectionString = BuildConnectionString();
        }

        #endregion

        #region Methods

        private static string BuildConnectionString()
        {
            string server = SettingsManager.Instance.GetSetting("Database", "DBServer").GetString();
            int port = SettingsManager.Instance.GetSetting("Database", "DBServerPort").GetInt32();
            string dbName = SettingsManager.Instance.GetSetting("Database", "DBName").GetString();
            string uid = SettingsManager.Instance.GetSetting("Database", "UserID").GetString();
            string pwd = SettingsManager.Instance.GetSetting("Database", "UserPWD").GetString();

            StringBuilder sb = new StringBuilder("metadata=res://*/Data.Entities.csdl|res://*/Data.Entities.ssdl|res://*/Data.Entities.msl;provider=MySql.Data.MySqlClient;provider connection string=\"server={SERVER};Port={PORT};User Id={UID};Password={PWD};database={DATABASE};Persist Security Info=True\"");
            sb.Replace("{SERVER}", server);
            sb.Replace("{PORT}", port.ToString());
            sb.Replace("{DATABASE}", dbName);
            sb.Replace("{UID}", uid);
            sb.Replace("{PWD}", pwd);

            return sb.ToString();
        }

        /// <summary>
        /// Creates the database context.
        /// </summary>
        /// <returns></returns>
        internal static AlarmWorkflowEntities CreateContext()
        {
            return new AlarmWorkflowEntities(ConnectionString);
        }

        /// <summary>
        /// Ensures that we have database access to the configured server.
        /// If database access is not possible, consecutive attempts are made after waiting a little.
        /// </summary>
        internal static void EnsureDatabaseReachable()
        {
            while (!CheckDatabaseReachable())
            {
                Logger.Instance.LogFormat(LogType.Warning, typeof(AlarmWorkflowEntities), Properties.Resources.DatabaseNotReachableErrorMessage, CheckConnectionRetryTimeoutMs);

                Thread.Sleep(CheckConnectionRetryTimeoutMs);
            }
        }

        private static bool CheckDatabaseReachable()
        {
            using (AlarmWorkflowEntities entities = CreateContext())
            {
                try
                {
                    entities.Operations.Any();
                    return true;
                }
                catch (System.Exception)
                {
                    // Intentionally left blank --> database not reachable or other error.
                }
                return false;
            }
        }

        #endregion

    }
}