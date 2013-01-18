using System;

namespace AlarmWorkflow.Shared.Addressing.EntryObjects
{
    /// <summary>
    /// Represents a "Phone" entry in the address book.
    /// </summary>
    public class MobilePhoneEntryObject
    {
        #region Properties

        /// <summary>
        /// Gets/sets the phone number represented by this entry object.
        /// </summary>
        public string PhoneNumber { get; set; }

        #endregion
    }
}
