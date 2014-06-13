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

using System.Collections.Generic;
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.BackendService.ManagementContracts.Emk;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.ManagementContracts
{
    /// <summary>
    /// Defines operations for the resource management service.
    /// </summary>
    public interface IEmkServiceInternal : IInternalService
    {
        /// <summary>
        /// Returns all resources that are configured.
        /// </summary>
        /// <returns>All resources that are configured.</returns>
        IEnumerable<EmkResource> GetAllResources();
        /// <summary>
        /// Filters a given list of <see cref="OperationResource"/>s according to the configured EMK.
        /// </summary>
        /// <param name="resources">The source list of resources that shall be filtered.</param>
        /// <returns>A filtered list of <see cref="OperationResource"/>s that has been filtered according to the configured EMK.</returns>
        IEnumerable<OperationResource> GetFilteredResources(IEnumerable<OperationResource> resources);
    }
}
