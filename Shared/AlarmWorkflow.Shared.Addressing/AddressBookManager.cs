using System;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Shared.Addressing
{
    /// <summary>
    /// Provides access to the address book.
    /// </summary>
    public static class AddressBookManager
    {
        #region Singleton

        private static AddressBook _addressBook;

        public static AddressBook GetInstance()
        {
            if (_addressBook == null)
            {
                _addressBook = SettingsManager.Instance.GetSetting("Addressing", "AddressBook").GetValue<AddressBook>();
            }
            return _addressBook;
        }

        #endregion
    }
}
