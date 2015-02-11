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

using System.ServiceModel;
using AlarmWorkflow.Backend.ServiceContracts.Core;

namespace AlarmWorkflow.BackendService.DispositioningContracts
{
    /// <summary>
    /// Defines mechanisms for a service that allows for real-time dispositioning of resources.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IDispositioningServiceCallback))]
    public interface IDispositioningService : IExposedService
    {
        /// <summary>
        /// Returns the resource IDs of all resources dispatched for the given operation.
        /// </summary>
        /// <param name="operationId">The ID of the operation to get all dispatched resource IDs.</param>
        /// <returns>A string array containing resource IDs of all resources dispatched for the given operation.
        /// -or- an empty array, if there were no dispatched resources.</returns>
        [OperationContract()]
        [FaultContract(typeof(AlarmWorkflowFaultDetails))]
        string[] GetDispatchedResources(int operationId);
        /// <summary>
        /// Enlists the given resource as dispositioned for the given operation.
        /// </summary>
        /// <param name="operationId">The ID of the operation affected.</param>
        /// <param name="emkResourceId">The ID of the EMK resource to disposition.</param>
        [OperationContract()]
        [FaultContract(typeof(AlarmWorkflowFaultDetails))]
        void Dispatch(int operationId, string emkResourceId);
        /// <summary>
        /// Cancels the dispositioning of the given EMK resource.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>Calling this method will remove the given resource from the dispositioned resources
        /// of the affected operation.</remarks>
        /// <param name="operationId">The ID of the operation affected.</param>
        /// <param name="emkResourceId">The ID of the EMK resource to recall.</param>
        [OperationContract()]
        [FaultContract(typeof(AlarmWorkflowFaultDetails))]
        void Recall(int operationId, string emkResourceId);
    }
}
