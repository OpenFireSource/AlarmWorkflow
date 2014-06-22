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
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.BackendService.ManagementContracts;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.Management
{
    class OperationService : ExposedCallbackServiceBase<IOperationServiceCallback>, IOperationService
    {
        #region Properties

        private IOperationServiceInternal InternalService
        {
            get { return ServiceProvider.GetService<IOperationServiceInternal>(); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationService"/> class.
        /// </summary>
        public OperationService()
        {
            InternalService.OperationAcknowledged += InternalService_OperationAcknowledged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void DisposeCore()
        {
            base.DisposeCore();

            InternalService.OperationAcknowledged -= InternalService_OperationAcknowledged;
        }

        private void InternalService_OperationAcknowledged(int id)
        {
            if (Callback != null)
            {
                Callback.OnOperationAcknowledged(id);
            }
        }

        #endregion

        #region IOperationService Members

        IList<int> IOperationService.GetOperationIds(int maxAge, bool onlyNonAcknowledged, int limitAmount)
        {
            try
            {
                return InternalService.GetOperationIds(maxAge, onlyNonAcknowledged, limitAmount);
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        Operation IOperationService.GetOperationById(int operationId)
        {
            try
            {
                return InternalService.GetOperationById(operationId);
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        void IOperationService.AcknowledgeOperation(int operationId)
        {
            try
            {
                InternalService.AcknowledgeOperation(operationId);
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        #endregion
    }
}