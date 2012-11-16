using System.Collections.Generic;

namespace AlarmWorkflow.Shared.Addressing
{
    /// <summary>
    /// Represents a person within the address book.
    /// </summary>
    public class AddressBookEntry
    {
        #region Properties

        /// <summary>
        /// Gets/sets the name of the recipient.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets/sets the dictionary containing custom data, which is specific by provider.
        /// </summary>
        public IDictionary<string, object> CustomData { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressBookEntry"/> class.
        /// </summary>
        public AddressBookEntry()
        {
            CustomData = new Dictionary<string, object>();
        }

        #endregion
    }
}
