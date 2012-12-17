using System;
using System.Data.Objects;
using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.MySqlDatabaseJob
{
    static class Helpers
    {
        /// <summary>
        /// Creates the database from the connection string which is embedded into "app.config".
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static T CreateContext<T>() where T : ObjectContext
        {
            try
            {
                string appConfigText = typeof(T).Assembly.GetEmbeddedResourceText("App.Config");

                XDocument appConfig = XDocument.Parse(appConfigText);

                XElement connectionStrings = appConfig.Root.Element("connectionStrings");

                // get first connection string
                XElement connectionStringE = connectionStrings.Elements("add").FirstOrDefault();

                string name = connectionStringE.Attribute("name").Value;
                string connectionString = connectionStringE.Attribute("connectionString").Value;

                return (T)Activator.CreateInstance(typeof(T), connectionString);
            }
            catch (Exception ex)
            {
                PrintException(ex);
                throw;
            }
        }

        private static void PrintException(Exception root)
        {
            Logger.Instance.LogFormat(LogType.Error, typeof(Helpers), "Exception while creating context: ", root.Message);
            if (root.InnerException != null)
            {
                PrintException(root.InnerException);
            }
        }
    }
}
