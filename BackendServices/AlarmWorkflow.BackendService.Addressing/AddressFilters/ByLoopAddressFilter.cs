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
using System.Linq;
using AlarmWorkflow.BackendService.AddressingContracts;
using AlarmWorkflow.BackendService.AddressingContracts.EntryObjects;
using AlarmWorkflow.BackendService.AddressingContracts.Extensibility;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.Addressing.AddressFilter
{
    /// <summary>
    /// Implements a custom <see cref="IAddressFilter"/> to query an entry based on the loops in the entry.
    /// </summary>
    [Export("ByLoopAddressFilter", typeof(IAddressFilter))]
    [Information(DisplayName = "ByLoopAddressFilterDisplayName", Description = "ByLoopAddressFilterDescription")]
    public class ByLoopAddressFilter : IAddressFilter
    {
        #region IAddressFilter Members

        bool IAddressFilter.QueryAcceptEntry(Operation operation, AddressBookEntry entry)
        {
            if (operation.Loops.Count == 0)
            {
                return true;
            }

            IEnumerable<LoopEntryObject> leo = entry.GetDataItems<LoopEntryObject>(LoopEntryObject.TypeId);
            IEnumerable<string> loops = leo.Select(eo => eo.Loop);

            return loops.Intersect(operation.Loops).Any();
        }

        #endregion
    }
}
