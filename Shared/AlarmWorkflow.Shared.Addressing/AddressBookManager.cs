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
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Shared.Addressing
{
    /// <summary>
    /// Provides access to the address book.
    /// </summary>
    public static class AddressBookManager
    {
        private static readonly Lazy<AddressBook> _addressBook;

        /// <summary>
        /// Initializes the <see cref="AddressBookManager"/> class.
        /// </summary>
        static AddressBookManager()
        {
            _addressBook = new Lazy<AddressBook>(() => SettingsManager.Instance.GetSetting("Addressing", "AddressBook").GetValue<AddressBook>(), true);
        }

        /// <summary>
        /// Returns the <see cref="AddressBook"/>-instance of the current AppDomain.
        /// The address book is created on-demand and is then cached.
        /// </summary>
        /// <returns>The <see cref="AddressBook"/>-instance of the current AppDomain.</returns>
        public static AddressBook GetInstance()
        {
            return _addressBook.Value;
        }
    }
}