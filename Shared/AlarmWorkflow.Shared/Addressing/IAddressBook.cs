using System;
using System.Collections.Generic;

namespace AlarmWorkflow.Shared.Addressing
{
    /// <summary>
    /// Defines a means for an address book that stores various data about persons.
    /// </summary>
    public interface IAddressBook
    {
        /// <summary>
        /// Enumerates through all available address book entries.
        /// </summary>
        /// <returns></returns>
        IEnumerable<AddressBookEntry> GetEntries();
        /// <summary>
        /// Returns the desired custom objects of all entries for the given type key.
        /// </summary>
        /// <typeparam name="TCustomData">The type of the custom data to project.</typeparam>
        /// <param name="type">The type key of the custom data to return.</param>
        /// <returns>A tuple that contains the entry and the custom data of that entry.</returns>
        IEnumerable<Tuple<AddressBookEntry, TCustomData>> GetCustomObjects<TCustomData>(string type);
    }
}
