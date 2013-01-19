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
