using System;
using System.Collections.Generic;

namespace AlarmWorkflow.Shared.Addressing
{
    /// <summary>
    /// Represents a person within the address book.
    /// </summary>
    public class AddressBookEntry : IEquatable<AddressBookEntry>
    {
        #region Properties

        /// <summary>
        /// Gets/sets the name of the recipient.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets/sets the dictionary containing custom data, which is specific by provider.
        /// </summary>
        public IList<EntryObject> Data { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressBookEntry"/> class.
        /// </summary>
        public AddressBookEntry()
        {
            Data = new List<EntryObject>();
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
            return string.Equals(this.Name, other.Name, StringComparison.Ordinal);
        }

        #endregion
    }
}
