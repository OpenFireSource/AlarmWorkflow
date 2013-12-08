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
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.AddressingContracts
{
    /// <summary>
    /// Defines the methods for the addressing service.
    /// </summary>
    public interface IAddressingServiceInternal : IInternalService
    {
        /// <summary>
        /// Returns a list containing all address book entries.
        /// </summary>
        /// <returns>A list containing all address book entries.</returns>
        IList<AddressBookEntry> GetAllEntries();
        /// <summary>
        /// Performs a query over all entries in this instance and returns all entries including their data items of the given type.
        /// </summary>
        /// <typeparam name="TCustomData">The custom data to expect.</typeparam>
        /// <param name="type">The type to query.</param>
        /// <returns>An enumerable of tuples that contain both the entry and the custom data of this entry.</returns>
        IEnumerable<Tuple<AddressBookEntry, TCustomData>> GetCustomObjects<TCustomData>(string type);
        /// <summary>
        /// Performs a query over all entries in this instance and returns all entries including their data items of the given type.
        /// Includes only entries that successfully match all the <see cref="Extensibility.IAddressFilter"/>s specified in the configuration.
        /// </summary>
        /// <typeparam name="TCustomData">The custom data to expect.</typeparam>
        /// <param name="type">The type to query.</param>
        /// <param name="operation">The <see cref="Operation"/> to use for filtering. Using null performs no filtering.</param>
        /// <returns>An enumerable of tuples that contain both the entry and the custom data of this entry.</returns>
        IEnumerable<Tuple<AddressBookEntry, TCustomData>> GetCustomObjectsFiltered<TCustomData>(string type, Operation operation);
    }
}
