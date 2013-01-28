﻿using System.Text;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;
using MySql.Data.MySqlClient;

namespace AlarmWorkflow.Job.MySqlDatabaseJob
{
    /// <summary>
    /// Legacy MySql-Database Job that writes to the "tb_einsatz" table.
    /// </summary>
    [Export("MySqlDatabaseJob", typeof(IJob))]
    sealed class MySqlDatabaseJob : IJob
    {
        #region Constants

        private const string TableName = "tb_einsatz";

        #endregion

        #region Fields

        private string _user;
        private string _password;
        private string _databaseName;
        private string _serverName;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DatabaseJob class.
        /// </summary>
        public MySqlDatabaseJob()
        {
        }

        #endregion

        #region Methods

        private MySqlConnection CreateConnection()
        {
            return new MySqlConnection("Persist Security Info=False;database=" + this._databaseName + ";server=" + this._serverName + ";user id=" + this._user + ";Password=" + this._password);
        }

        #endregion

        #region IJob Members

        bool IJob.Initialize()
        {
            _databaseName = SettingsManager.Instance.GetSetting("MySqlDatabaseJob", "DBName").GetString();
            _user = SettingsManager.Instance.GetSetting("MySqlDatabaseJob", "UserID").GetString();
            _password = SettingsManager.Instance.GetSetting("MySqlDatabaseJob", "UserPWD").GetString();
            _serverName = SettingsManager.Instance.GetSetting("MySqlDatabaseJob", "DBServer").GetString();

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

        void IJob.Execute(IJobContext context, Operation operation)
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
                queryText.Append("(Einsatznr, Einsatzort, Einsatzplan, Hinweis, Kreuzung, Meldebild, Mitteiler, Objekt, Ort, Strasse, Fahrzeuge, Alarmtime, Faxtime, Einsatzstichwort, Stichwort) ");
                queryText.Append("VALUES (");
                queryText.AppendFormat("'{0}', ", operation.OperationNumber);
                queryText.AppendFormat("'{0}', ", operation.Einsatzort.Location);
                queryText.AppendFormat("'{0}', ", operation.OperationPlan);
                queryText.AppendFormat("'{0}', ", operation.Comment);
                queryText.AppendFormat("'{0}', ", operation.Einsatzort.Intersection);
                queryText.AppendFormat("'{0}', ", operation.Picture);
                queryText.AppendFormat("'{0}', ", operation.Messenger);
                queryText.AppendFormat("'{0}', ", operation.Einsatzort.Property);
                queryText.AppendFormat("'{0}', ", operation.Einsatzort.City);
                queryText.AppendFormat("'{0}', ", operation.Einsatzort.Street + " " + operation.Einsatzort.StreetNumber);
                queryText.AppendFormat("'{0}', ", operation.Resources.ToString("{FullName} {RequestedEquipment} | ", null));
                queryText.AppendFormat("'{0}', ", operation.GetCustomData<string>("Alarmtime"));
                queryText.AppendFormat("'{0}', ", operation.GetCustomData<string>("Faxtime"));
                queryText.AppendFormat("'{0}', ", operation.Keywords.EmergencyKeyword);
                queryText.AppendFormat("'{0}'", operation.Keywords.Keyword);
                queryText.Append(")");

                MySqlCommand cmd = new MySqlCommand(queryText.ToString(), conn);
                cmd.ExecuteNonQuery();
            }
        }

        bool IJob.IsAsync
        {
            get { return false; }
        }

        #endregion

        #region IDisposable Members

        void System.IDisposable.Dispose()
        {

        }

        #endregion
    }
}
