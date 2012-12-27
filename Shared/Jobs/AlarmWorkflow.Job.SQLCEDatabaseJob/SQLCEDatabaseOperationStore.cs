using System;
using System.Collections.Generic;
using System.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
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

        Operation IOperationStore.StoreOperation(Operation operation)
        {
            lock (Lock)
            {
                try
                {
                    using (SQLCEDatabaseEntities entities = Helpers.CreateContext<SQLCEDatabaseEntities>())
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
                            ID = operation.OperationGuid,
                            OperationId = oid,
                            Timestamp = Helpers.EnsureSaneTimestamp(timestamp),
                            IsAcknowledged = operation.IsAcknowledged,
                            Serialized = Utilities.Serialize(operation),
                        };
                        entities.Operations.AddObject(data);
                        entities.SaveChanges();

                        // Update Operation ID afterwards
                        operation.Id = oid;
                        return operation;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, "An error occurred while trying to write the operation to the database!");
                    Logger.Instance.LogException(this, ex);
                    throw ex;
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

                    Operation operation = Utilities.Deserialize<Operation>(data.Serialized);
                    operation.Id = data.OperationId;
                    operation.Timestamp = data.Timestamp;

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