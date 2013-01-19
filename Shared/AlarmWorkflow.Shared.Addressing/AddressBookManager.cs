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
