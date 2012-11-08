using System;
using System.Linq;
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
                        OperationId = oid,
                        Timestamp = Helpers.EnsureSaneTimestamp(timestamp),
                        City = operation.City,
                        ZipCode = operation.ZipCode,
                        Location = operation.Location,
                        OperationNumber = operation.OperationNumber,
                        Keyword = operation.Keyword,
                        Comment = operation.Comment,
                        IsAcknowledged = operation.IsAcknowledged,
                        Messenger = operation.Messenger,
                        Building = operation.Property,
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
    }
}
