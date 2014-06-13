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
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.Backend.ServiceContracts.Data;
using AlarmWorkflow.BackendService.Management.Data;
using AlarmWorkflow.BackendService.ManagementContracts;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.Management
{
    class OperationServiceInternal : InternalServiceBase, IOperationServiceInternal
    {
        #region Constants

        private const string EdmxPath = "Data.Entities";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationServiceInternal"/> class.
        /// </summary>
        public OperationServiceInternal()
        {
        }

        #endregion

        #region IOperationServiceInternal Members

        /// <summary>
        /// Raised when an operation was acknowledged.
        /// </summary>
        public event Action<int> OperationAcknowledged;

        bool IOperationServiceInternal.ExistsOperation(string operationNumber)
        {
            if (string.IsNullOrWhiteSpace(operationNumber))
            {
                return false;
            }

            using (OperationManagementEntities entities = EntityFrameworkHelper.CreateContext<OperationManagementEntities>(EdmxPath))
            {
                return entities.Operations.Any(item => item.OperationNumber == operationNumber);
            }
        }

        IList<int> IOperationServiceInternal.GetOperationIds(int maxAge, bool onlyNonAcknowledged, int limitAmount)
        {
            List<int> operations = new List<int>();

            using (OperationManagementEntities entities = EntityFrameworkHelper.CreateContext<OperationManagementEntities>(EdmxPath))
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

        Operation IOperationServiceInternal.GetOperationById(int operationId)
        {
            List<Operation> operations = new List<Operation>();

            using (OperationManagementEntities entities = EntityFrameworkHelper.CreateContext<OperationManagementEntities>(EdmxPath))
            {
                OperationData data = entities.Operations.FirstOrDefault(d => d.Id == operationId);
                if (data == null)
                {
                    return null;
                }

                return data.ToOperation();
            }
        }

        void IOperationServiceInternal.AcknowledgeOperation(int operationId)
        {
            using (OperationManagementEntities entities = EntityFrameworkHelper.CreateContext<OperationManagementEntities>(EdmxPath))
            {
                OperationData data = entities.Operations.FirstOrDefault(d => d.Id == operationId);
                if (data == null || data.IsAcknowledged)
                {
                    return;
                }

                data.IsAcknowledged = true;
                entities.SaveChanges();
            }

            var copy = OperationAcknowledged;
            if (copy != null)
            {
                copy(operationId);
            }
        }

        Operation IOperationServiceInternal.StoreOperation(Operation operation)
        {
            using (OperationManagementEntities entities = EntityFrameworkHelper.CreateContext<OperationManagementEntities>(EdmxPath))
            {
                OperationData data = new OperationData(operation);
                entities.Operations.AddObject(data);
                entities.SaveChanges();

                operation.Id = data.Id;
                return operation;
            }
        }

        #endregion
    }
}