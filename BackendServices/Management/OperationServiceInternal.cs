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
using AlarmWorkflow.Backend.Data;
using AlarmWorkflow.Backend.Data.Types;
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.BackendService.ManagementContracts;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.Management
{
    class OperationServiceInternal : InternalServiceBase, IOperationServiceInternal
    {
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

        /// <summary>
        /// Raised when an operation was added.
        /// </summary>
        public event Action<Operation> NewOperation;

        bool IOperationServiceInternal.ExistsOperation(string operationNumber)
        {
            if (string.IsNullOrWhiteSpace(operationNumber))
            {
                return false;
            }

            using (IUnitOfWork work = ServiceProvider.GetService<IDataContextFactory>().Get().Create())
            {
                return work.For<OperationData>().Query.Any(item => item.OperationNumber == operationNumber);
            }
        }

        IList<int> IOperationServiceInternal.GetOperationIds(int maxAge, bool onlyNonAcknowledged, int limitAmount)
        {
            List<int> operations = new List<int>();

            using (IUnitOfWork work = ServiceProvider.GetService<IDataContextFactory>().Get().Create())
            {
                foreach (OperationData data in work.For<OperationData>().Query.OrderByDescending(o => o.IncomeAt))
                {
                    // If we only want non-acknowledged ones
                    if (onlyNonAcknowledged && data.IsAcknowledged)
                    {
                        continue;
                    }
                    // If we shall ignore the age, or obey the maximum age...
                    if (maxAge > 0 && (DateTime.Now - data.IncomeAt).TotalMinutes > maxAge)
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

            using (IUnitOfWork work = ServiceProvider.GetService<IDataContextFactory>().Get().Create())
            {
                OperationData data = work.For<OperationData>().Query.FirstOrDefault(d => d.Id == operationId);
                if (data == null)
                {
                    return null;
                }

                return data.ToOperation();
            }
        }

        void IOperationServiceInternal.AcknowledgeOperation(int operationId)
        {
            using (IUnitOfWork work = ServiceProvider.GetService<IDataContextFactory>().Get().Create())
            {
                OperationData data = work.For<OperationData>().Query.FirstOrDefault(d => d.Id == operationId);
                if (data == null || data.IsAcknowledged)
                {
                    return;
                }

                data.IsAcknowledged = true;

                work.Commit();
            }

            var copy = OperationAcknowledged;
            if (copy != null)
            {
                copy(operationId);
            }
        }

        Operation IOperationServiceInternal.StoreOperation(Operation operation)
        {
            using (IUnitOfWork work = ServiceProvider.GetService<IDataContextFactory>().Get().Create())
            {
                OperationData data = operation.ToData();

                work.For<OperationData>().Insert(data);

                work.Commit();

                operation.Id = data.Id;
            }

            var copy = NewOperation;
            if (copy != null)
            {
                copy(operation);
            }
            return operation;
        }

        #endregion
    }
}