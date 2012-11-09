using System.Text;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;
using MySql.Data.MySqlClient;

// TODO: needs to be updated!!!

namespace AlarmWorkflow.Job.MySqlDatabaseJob
{
    /// <summary>
    /// Implements a job, who saves all the operation data to a MySQL database.
    /// </summary>
    [Export("MySqlDatabaseJob", typeof(IJob))]
    sealed class DatabaseJob : IJob
    {
        #region Constants

        private const string TableName = "tb_einsatz";

        #endregion

        #region private members

        private string user;
        private string pwd;
        private string database;
        private string server;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DatabaseJob class.
        /// </summary>
        public DatabaseJob()
        {
        }

        #endregion

        #region Methods

        private MySqlConnection CreateConnection()
        {
            return new MySqlConnection("Persist Security Info=False;database=" + this.database + ";server=" + this.server + ";user id=" + this.user + ";Password=" + this.pwd);
        }

        #endregion

        #region IJob Members

        bool IJob.Initialize()
        {
            this.database = SettingsManager.Instance.GetSetting("MySqlDatabaseJob", "DBName").GetString();
            this.user = SettingsManager.Instance.GetSetting("MySqlDatabaseJob", "UserID").GetString();
            this.pwd = SettingsManager.Instance.GetSetting("MySqlDatabaseJob", "UserPWD").GetString();
            this.server = SettingsManager.Instance.GetSetting("MySqlDatabaseJob", "DBServer").GetString();

            // Check whether we can connect properly...
            try
            {
                using (MySqlConnection conn = CreateConnection())
                {
                    conn.Open();
                }
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }

        void IJob.DoJob(Operation einsatz)
        {
            using (MySqlConnection conn = CreateConnection())
            {
                conn.Open();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, "Could not open SQL Connection!");
                    return;
                }

                // TODO: This string contains CustomData. When actually using this job this should be revised to NOT use any custom data (or make it extensible)!
                StringBuilder queryText = new StringBuilder();
                queryText.AppendFormat("INSERT INTO {0} ", TableName);
                queryText.Append("(Einsatznr, Einsatzort, Einsatzplan, Hinweis, Kreuzung, Meldebild, Mitteiler, Objekt, Ort, Strasse, Stichwort) ");
                queryText.Append("VALUES (");
                queryText.AppendFormat("'{0}', ", einsatz.OperationNumber);
                queryText.AppendFormat("'{0}', ", einsatz.Location);
                queryText.AppendFormat("'{0}', ", einsatz.CustomData["Einsatzplan"]);
                queryText.AppendFormat("'{0}', ", einsatz.Comment);
                queryText.AppendFormat("'{0}', ", einsatz.CustomData["Kreuzung"]);
                queryText.AppendFormat("'{0}', ", einsatz.CustomData["Meldebild"]);
                queryText.AppendFormat("'{0}', ", einsatz.Messenger);
                queryText.AppendFormat("'{0}', ", einsatz.Property);
                queryText.AppendFormat("'{0}', ", einsatz.City);
                queryText.AppendFormat("'{0}', ", einsatz.Street);
                queryText.AppendFormat("'{0}'", einsatz.Keyword);
                queryText.Append(")");

                MySqlCommand cmd = new MySqlCommand(queryText.ToString(), conn);
                cmd.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
