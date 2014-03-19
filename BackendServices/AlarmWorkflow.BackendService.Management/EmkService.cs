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
using AlarmWorkflow.BackendService.ManagementContracts;
using AlarmWorkflow.BackendService.ManagementContracts.Emk;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.Management
{
    class EmkService : ExposedServiceBase, IEmkService
    {
        #region IEmkService Members

        IList<EmkResource> IEmkService.GetAllResources()
        {
            try
            {
                return this.ServiceProvider.GetService<IEmkServiceInternal>().GetAllResources().ToList();
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        IList<OperationResource> IEmkService.GetFilteredResources(IList<OperationResource> resources)
        {
            try
            {
                return this.ServiceProvider.GetService<IEmkServiceInternal>().GetFilteredResources(resources).ToList();
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        #endregion
    }
}
