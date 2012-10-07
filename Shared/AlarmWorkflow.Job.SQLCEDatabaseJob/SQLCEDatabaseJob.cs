using System;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.SQLCEDatabaseJob
{
    [Export("SQLCEDatabaseJob", typeof(IJob))]
    class SQLCEDatabaseJob : IJob
    {
        #region IJob Member

        void IJob.DoJob(Operation operation)
        {
            try
            {
                using (SQLCEDatabaseEntities entities = this.CreateContext<SQLCEDatabaseEntities>())
                {
                    int oid = operation.Id;
                    if (operation.Id == 0)
                    {
                        oid = entities.Operations.Any() ? entities.Operations.Max(o => o.OperationId) + 1 : 1;
                    }

                    // We need to see if the timestamp could be parsed. It will cause a Overflow in SQL Server if we allow DateTime.MinValue!
                    DateTime timestamp = (operation.Timestamp != DateTime.MinValue) ? operation.Timestamp : DateTime.Now;

                    OperationData data = new OperationData()
                    {
                        OperationId = oid,
                        Timestamp = timestamp,
                        City = operation.City,
                        ZipCode = operation.ZipCode,
                        Location = operation.Location,
                        OperationNumber = operation.OperationNumber,
                        Keyword = operation.Keyword,
                        Comment = operation.Comment,
                        IsAcknowledged = operation.IsAcknowledged,
                        Messenger = operation.Messenger,
                        Property = operation.Property,
                        Street = operation.Street,
                        StreetNumber = operation.StreetNumber,
                        CustomData = Utilities.Serialize(operation.CustomData),
                        // TODO: Compress route image!?
                        RouteImage = operation.RouteImage,
                    };
                    entities.Operations.AddObject(data);
                    entities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, "An error occurred while trying to write the operation to the database!");
                Logger.Instance.LogException(this, ex);
            }
        }

        bool IJob.Initialize()
        {
            return true;
        }

        #endregion

        #region Methods

        private T CreateContext<T>() where T : ObjectContext
        {
            try
            {
                string resourceName = this.GetType().Assembly.GetName().Name + ".app.config";
                using (Stream stream = this.GetType().Assembly.GetManifestResourceStream(resourceName))
                {

                    XDocument appConfig = XDocument.Load(stream);

                    XElement connectionStrings = appConfig.Root.Element("connectionStrings");

                    // get first connection string
                    XElement connectionStringE = connectionStrings.Elements("add").Where(n => n.Attribute("name").Value == "SQLCEDatabaseEntities").FirstOrDefault();

                    string name = connectionStringE.Attribute("name").Value;
                    string connectionString = connectionStringE.Attribute("connectionString").Value;

                    return (T)Activator.CreateInstance(typeof(T), connectionString);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
