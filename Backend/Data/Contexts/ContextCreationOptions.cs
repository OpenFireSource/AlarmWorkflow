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

using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using System;
using System.Linq;
using System.Threading;

namespace AlarmWorkflow.Backend.Data.Contexts
{
    sealed class ContextCreationOptions
    {
        #region Enums

        public enum DatabaseEngine { MySQL, SQLite }

        #endregion

        #region Constants

        private const string ConnectionStringTemplateMySQL = "server={0};Port={1};User Id={2};Password={3};database={4};Persist Security Info=True";

        private const string ConnectionStringTemplateSQLite = "Data Source={0};";

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the host name of the server to connect to.
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// Gets/sets the host name of the server to connect to.
        /// </summary>
        public DatabaseEngine Engine { get; set; }
        /// <summary>
        /// Gets/sets the port of the server to connect to.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Gets/sets the name of the user for logging in.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Gets/sets the password of the user.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Gets/sets the name of the database to connect to.
        /// </summary>
        public string DatabaseName { get; set; }
        /// <summary>
        /// Gets/sets the name of the database path to connect to.
        /// </summary>
        public string SQLiteDatabase { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the connection string from the values of this instancefor a MySQL database.
        /// </summary>
        /// <returns></returns>
        public string GetMySqlConnectionString()
        {
            return string.Format(ConnectionStringTemplateMySQL, HostName, Port, UserId, Password, DatabaseName);
        }

        /// <summary>
        /// Returns the connection string from the values of this instance for a sqlite database.
        /// </summary>
        /// <returns></returns>
        public string GetSQLiteConnectionString()
        {
            return string.Format(ConnectionStringTemplateSQLite, SQLiteDatabase);
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// Creates the <see cref="ContextCreationOptions"/> and uses the values from the backend configuration.
        /// </summary>
        /// <returns>An instance of <see cref="ContextCreationOptions"/> which uses the values from the backend configuration.</returns>
        public static ContextCreationOptions CreateFromSettings()
        {
            var options = new ContextCreationOptions
            {
                Engine = GetEngine(),
                HostName = ServiceFactory.BackendConfigurator.Get("Server.DB.HostName"),
                Port = int.Parse(ServiceFactory.BackendConfigurator.Get("Server.DB.Port")),
                UserId = ServiceFactory.BackendConfigurator.Get("Server.DB.UserId"),
                Password = ServiceFactory.BackendConfigurator.Get("Server.DB.Password"),
                DatabaseName = ServiceFactory.BackendConfigurator.Get("Server.DB.DatabaseName"),
                SQLiteDatabase = Utilities.GetLocalAppDataFolderFileName(ServiceFactory.BackendConfigurator.Get("Server.DB.SQLite.Database"))
            };
            return options;
        }

        private static DatabaseEngine GetEngine()
        {
            string engineName = ServiceFactory.BackendConfigurator.Get("Server.DB.Engine");

            try
            {
                return (DatabaseEngine)Enum.Parse(typeof(DatabaseEngine), engineName, true);
            }
            catch (ArgumentException)
            {
                Logger.Instance.LogFormat(LogType.Warning, typeof(ContextCreationOptions), Properties.Resources.EngineNotFound, engineName, "MySQL");
                return DatabaseEngine.MySQL;
            }
        }

        #endregion
    }
}
