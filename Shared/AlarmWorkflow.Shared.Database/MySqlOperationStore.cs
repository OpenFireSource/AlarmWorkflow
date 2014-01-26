// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Database.Data;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Shared.Database
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
            AlarmWorkflowEntities.EnsureDatabaseReachable();
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
                    if (data == null || data.IsAcknowledged)
                    {
                        return;
                    }

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

                    return data.ToOperation();
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
                    foreach (OperationData data in entities.Operations.OrderByDescending(o => o.TimestampIncome))
                    {
                        // If we only want non-acknowledged ones
                        if (onlyNonAcknowledged && data.IsAcknowledged)
                        {
                            continue;
                        }
                        // If we shall ignore the age, or obey the maximum age...
                        if (maxAge > 0 && (DateTime.Now - data.TimestampIncome).TotalMinutes > maxAge)
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
                    OperationData data = new OperationData(operation);
                    entities.Operations.AddObject(data);
                    entities.SaveChanges();

                    operation.Id = data.Id;
                    return operation;
                }
            }
        }

        #endregion
    }
}