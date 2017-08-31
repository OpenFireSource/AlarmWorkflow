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
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.BackendService.DispositioningContracts;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.Dispositioning
{
    class DispositioningService : ExposedCallbackServiceBase<IDispositioningServiceCallback>, IDispositioningService
    {
        #region Properties

        private IDispositioningServiceInternal InternalService => ServiceProvider.GetService<IDispositioningServiceInternal>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DispositioningService"/> class.
        /// </summary>
        public DispositioningService()
        {
            InternalService.Dispositioning += InternalService_Dispositioning;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void DisposeCore()
        {
            base.DisposeCore();

            InternalService.Dispositioning -= InternalService_Dispositioning;
        }

        private void InternalService_Dispositioning(object sender, DispositionEventArgs e)
        {
            if (Callback != null)
            {
                Callback.OnEvent(e);
            }
        }

        #endregion

        #region IDispositioningService Members

        string[] IDispositioningService.GetDispatchedResources(int operationId)
        {
            try
            {
                return InternalService.GetDispatchedResources(operationId);
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        void IDispositioningService.Dispatch(int operationId, string emkResourceId)
        {
            try
            {
                InternalService.Dispatch(operationId, emkResourceId);
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        void IDispositioningService.Recall(int operationId, string emkResourceId)
        {
            try
            {
                InternalService.Recall(operationId, emkResourceId);
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        #endregion
    }
}
