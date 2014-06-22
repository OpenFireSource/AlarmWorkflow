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
using AlarmWorkflow.BackendService.AddressingContracts;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.Addressing
{
    class AddressingService : ExposedServiceBase, IAddressingService
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressingService"/> class.
        /// </summary>
        public AddressingService()
            : base()
        {

        }

        #endregion

        #region IAddressingService Members

        IList<AddressBookEntry> IAddressingService.GetAllEntries()
        {
            try
            {
                return this.ServiceProvider.GetService<IAddressingServiceInternal>().GetAllEntries();
            }
            catch (Exception ex)
            {
                throw AlarmWorkflowFaultDetails.CreateFault(ex);
            }
        }

        #endregion
    }
}
