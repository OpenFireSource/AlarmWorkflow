using System.Linq;
using System.Text;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Shared.Database.Data
{
    partial class AlarmWorkflowEntities
    {
        private static readonly string ConnectionString;

        static AlarmWorkflowEntities()
        {
            ConnectionString = BuildConnectionString();
        }

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

        internal static bool CheckDatabaseReachable()
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
    }
}
