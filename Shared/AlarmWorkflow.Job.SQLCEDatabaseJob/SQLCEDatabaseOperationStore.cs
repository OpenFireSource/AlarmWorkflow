using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.SQLCEDatabaseJob
{
    [Export("SQLCEDatabaseOperationStore", typeof(IOperationStore))]
    class SQLCEDatabaseOperationStore : IOperationStore
    {
        #region Fields

        private readonly object Lock = new object();

        #endregion

        #region IOperationStore Member

        int IOperationStore.GetNextOperationId()
        {
            lock (Lock)
            {
                using (SQLCEDatabaseEntities entities = CreateContext<SQLCEDatabaseEntities>())
                {
                    if (entities.Operations.Any())
                    {
                        return entities.Operations.Max(o => o.OperationId) + 1;
                    }
                    return 1;
                }
            }
        }

        void IOperationStore.AcknowledgeOperation(int operationId)
        {
            lock (Lock)
            {
                using (SQLCEDatabaseEntities entities = CreateContext<SQLCEDatabaseEntities>())
                {
                    OperationData data = entities.Operations.FirstOrDefault(d => d.OperationId == operationId);
                    // If either there is no operation by this id, or the operation exists and is already acknowledged, do nothing
                    if (data == null || data.IsAcknowledged)
                    {
                        return;
                    }

                    // Acknowledge this operation and save changes
                    data.IsAcknowledged = true;
                    entities.SaveChanges();
                }
            }
        }

        Operation IOperationStore.GetOperationById(int operationId)
        {
            lock (Lock)
            {
                List<Operation> operations = new List<Operation>();

                using (SQLCEDatabaseEntities entities = CreateContext<SQLCEDatabaseEntities>())
                {
                    OperationData data = entities.Operations.FirstOrDefault(d => d.OperationId == operationId);
                    if (data == null)
                    {
                        return null;
                    }

                    return new Operation()
                    {
                        Id = data.OperationId,
                        Timestamp = data.Timestamp,
                        City = data.City,
                        IsAcknowledged = data.IsAcknowledged,
                        Keyword = data.Keyword,
                        Comment = data.Comment,
                        Location = data.Location,
                        Messenger = data.Messenger,
                        OperationNumber = data.OperationNumber,
                        Property = data.Building,
                        Street = data.Street,
                        StreetNumber = data.StreetNumber,
                        ZipCode = data.ZipCode,
                        CustomData = Utilities.Deserialize<IDictionary<string, object>>(data.CustomData),
                        RouteImage = data.RouteImage,
                    };
                }
            }
        }

        IList<int> IOperationStore.GetOperationIds(int maxAge, bool onlyNonAcknowledged, int limitAmount)
        {
            lock (Lock)
            {
                List<int> operations = new List<int>();

                using (SQLCEDatabaseEntities entities = CreateContext<SQLCEDatabaseEntities>())
                {
                    foreach (OperationData data in entities.Operations.OrderByDescending(o => o.Timestamp))
                    {
                        // If we only want non-acknowledged ones
                        if (onlyNonAcknowledged && data.IsAcknowledged)
                        {
                            continue;
                        }
                        // If we shall ignore the age, or obey the maximum age...
                        if (maxAge > 0 && (DateTime.Now - data.Timestamp).TotalDays > maxAge)
                        {
                            continue;
                        }

                        operations.Add(data.OperationId);

                        // If we need to limit operations
                        if (limitAmount > 0 && operations.Count >= limitAmount)
                        {
                            break;
                        }
                    }
                }

                return operations;
            }
        }

        #endregion

        #region Methods

        private T CreateContext<T>() where T : ObjectContext
        {
            try
            {
                string appConfigText = this.GetType().Assembly.GetEmbeddedResourceText("app.config");

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

        #endregion
    }
}