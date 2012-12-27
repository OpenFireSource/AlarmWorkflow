using System;
using System.Data.Objects;
using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Job.SQLCEDatabaseJob
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
                string appConfigText = typeof(T).Assembly.GetEmbeddedResourceText("app.config");

                XDocument appConfig = XDocument.Parse(appConfigText);

                XElement connectionStrings = appConfig.Root.Element("connectionStrings");

                // get first connection string
                XElement connectionStringE = connectionStrings.Elements("add").Where(n => n.Attribute("name").Value == "SQLCEDatabaseEntities").FirstOrDefault();

                string name = connectionStringE.Attribute("name").Value;
                string connectionString = connectionStringE.Attribute("connectionString").Value;

                return (T)Activator.CreateInstance(typeof(T), connectionString);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Ensures 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        internal static DateTime EnsureSaneTimestamp(DateTime timestamp)
        {
            // There is a problem when the date is BEFORE January 1, 1753 (basically all incorrectly or failed parsings).
            // See: http://msdn.microsoft.com/en-us/library/system.data.sqltypes.sqldatetime.minvalue.aspx
            // If we encounter such date, we simply use the current timestamp!

            if (timestamp.Year > 1753)
            {
                return timestamp;
            }

            return DateTime.Now;
        }
    }
}
