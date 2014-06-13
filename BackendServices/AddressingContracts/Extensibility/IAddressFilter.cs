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

using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.AddressingContracts.Extensibility
{
    /// <summary>
    /// Defines a method that queries one <see cref="AddressBookEntry"/> based on the context of an <see cref="Operation"/>.
    /// Allows for operation-specific determination of entries to use.
    /// </summary>
    public interface IAddressFilter
    {
        /// <summary>
        /// Represents a custom method to query one <see cref="AddressBookEntry"/> based on the context of an <see cref="Operation"/>.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/> to base the query on.</param>
        /// <param name="entry">The entry in question.</param>
        /// <returns>A boolean value indicating whether or not to accept this entry. Return true to accept this entry for retrieval; and false to deny it.</returns>
        bool QueryAcceptEntry(Operation operation, AddressBookEntry entry);
    }
}
