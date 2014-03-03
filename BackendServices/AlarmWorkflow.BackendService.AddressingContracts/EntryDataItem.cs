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

using System.Runtime.Serialization;
using AlarmWorkflow.BackendService.AddressingContracts.EntryObjects;

namespace AlarmWorkflow.BackendService.AddressingContracts
{
    /// <summary>
    /// An object that is a part of an <see cref="AddressBookEntry"/>.
    /// </summary>
    [DataContract()]
    [KnownType(typeof(LoopEntryObject))]
    [KnownType(typeof(MailAddressEntryObject))]
    [KnownType(typeof(MobilePhoneEntryObject))]
    [KnownType(typeof(PushEntryObject))]
    public class EntryDataItem
    {
        #region Properties

        /// <summary>
        /// Gets/sets the identifier of the object.
        /// </summary>
        [DataMember()]
        public string Identifier { get; set; }
        /// <summary>
        /// Gets/sets whether this entry data item is enabled.
        /// </summary>
        [DataMember()]
        public bool IsEnabled { get; set; }
        /// <summary>
        /// Gets/sets the data object that is represented by this instance.
        /// </summary>
        [DataMember()]
        public object Data { get; set; }

        #endregion
    }
}