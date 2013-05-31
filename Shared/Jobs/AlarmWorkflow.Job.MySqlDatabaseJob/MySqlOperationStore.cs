using System;
using System.Collections.Generic;
using System.Linq;
using AlarmWorkflow.Job.MySqlDatabaseJob.Data;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

////////////////////////////////
// TODO: Remove Operation.IsAcknowledged (redundant!)
// TODO: Remove Operation.DefaultAcknowledgingTimespan (not used and not necessary!)
// TODO: OperationData.Timestamp is redundant??? --> Keep for better performance (no deserialization needed)?
// TODO: OperationData.Timestamp shall be set to Operation.TimestampIncome!!!
// TODO: --> should all be done when updating tables!
////////////////////////////////

namespace AlarmWorkflow.Job.MySqlDatabaseJob
{
    [Export("MySqlOperationStore", typeof(IOperationStore))]
    class MySqlOperationStore : IOperationStore
    {
        #region Fields

        private readonly object Lock = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlOperationStore"/> class.
        /// </summary>
        public MySqlOperationStore()
        {

        }

        #endregion

        #region IOperationStore Members

        void IOperationStore.AcknowledgeOperation(int operationId)
        {
            lock (Lock)
            {
                using (AlarmWorkflowEntities entities = AlarmWorkflowEntities.CreateContext())
                {
                    OperationData data = entities.Operations.FirstOrDefault(d => d.Id == operationId);
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

                using (AlarmWorkflowEntities entities = AlarmWorkflowEntities.CreateContext())
                {
                    OperationData data = entities.Operations.FirstOrDefault(d => d.Id == operationId);
                    if (data == null)
                    {
                        return null;
                    }

                    Operation operation = Utilities.Deserialize<Operation>(data.Serialized);
                    operation.Id = data.Id;
                    operation.TimestampIncome = data.Timestamp;

                    return operation;
                }
            }
        }

        IList<int> IOperationStore.GetOperationIds(int maxAge, bool onlyNonAcknowledged, int limitAmount)
        {
            lock (Lock)
            {
                List<int> operations = new List<int>();

                using (AlarmWorkflowEntities entities = AlarmWorkflowEntities.CreateContext())
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

                        operations.Add(data.Id);

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

        Operation IOperationStore.StoreOperation(Operation operation)
        {
            lock (Lock)
            {
                using (AlarmWorkflowEntities entities = AlarmWorkflowEntities.CreateContext())
                {
                    OperationData data = new OperationData()
                    {
                        OperationId = operation.OperationGuid,
                        Timestamp = operation.TimestampIncome,
                        IsAcknowledged = operation.IsAcknowledged,
                        Serialized = Utilities.Serialize(operation),
                    };
                    entities.Operations.AddObject(data);
                    entities.SaveChanges();

                    // Update Operation ID afterwards
                    operation.Id = data.Id;
                    return operation;
                }
            }
        }

        #endregion
    }
}
