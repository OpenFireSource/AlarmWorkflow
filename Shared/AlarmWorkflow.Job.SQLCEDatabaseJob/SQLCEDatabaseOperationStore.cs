using System;
using System.Collections.Generic;
using System.Linq;
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
                using (SQLCEDatabaseEntities entities = Helpers.CreateContext<SQLCEDatabaseEntities>())
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
                using (SQLCEDatabaseEntities entities = Helpers.CreateContext<SQLCEDatabaseEntities>())
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

                using (SQLCEDatabaseEntities entities = Helpers.CreateContext<SQLCEDatabaseEntities>())
                {
                    OperationData data = entities.Operations.FirstOrDefault(d => d.OperationId == operationId);
                    if (data == null)
                    {
                        return null;
                    }

                    Operation operation = new Operation()
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

                    // There are new properties, which are unsure whether or not they are going to be added permantently.
                    // Thus we will add them to the CustomData until clarified.
                    operation.Picture = operation.GetCustomData<string>("Picture");
                    operation.EmergencyKeyword = operation.GetCustomData<string>("EmergencyKeyword");
                    operation.OperationPlan = operation.GetCustomData<string>("OperationPlan");

                    return operation;
                }
            }
        }

        IList<int> IOperationStore.GetOperationIds(int maxAge, bool onlyNonAcknowledged, int limitAmount)
        {
            lock (Lock)
            {
                List<int> operations = new List<int>();

                using (SQLCEDatabaseEntities entities = Helpers.CreateContext<SQLCEDatabaseEntities>())
                {
                    foreach (OperationData data in entities.Operations.OrderByDescending(o => o.Timestamp))
                    {
                        // If we only want non-acknowledged ones
                        if (onlyNonAcknowledged && data.IsAcknowledged)
                        {
                            continue;
                        }
                        // If we shall ignore the age, or obey the maximum age...
                        if (maxAge > 0 && (DateTime.Now - data.Timestamp).TotalMinutes > maxAge)
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
    }
}