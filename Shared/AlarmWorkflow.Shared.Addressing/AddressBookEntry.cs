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
using System.Diagnostics;

namespace AlarmWorkflow.Shared.Addressing
{
    /// <summary>
    /// Represents a person within the address book.
    /// </summary>
    [DebuggerDisplay("Name = {FirstName},{LastName}")]
    public sealed class AddressBookEntry : IEquatable<AddressBookEntry>
    {
        #region Properties

        /// <summary>
        /// Gets/sets the person's first name.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Gets/sets the person's last name.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Gets/sets the dictionary containing custom data, which is specific by provider.
        /// </summary>
        public IList<EntryDataItem> Data { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressBookEntry"/> class.
        /// </summary>
        public AddressBookEntry()
        {
            Data = new List<EntryDataItem>();
        }

        #endregion

        #region IEquatable<AddressBookEntry> Members

        /// <summary>
        /// Returns whether or not this instance is equal to another instance.
        /// Equality is determined by comparing the names.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AddressBookEntry other)
        {
            if (other == null)
            {
                return false;
            }
            return (this.FirstName == other.FirstName)
                && (this.LastName == other.LastName);
        }

        #endregion
    }
}