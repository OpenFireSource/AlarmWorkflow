using System;

namespace AlarmWorkflow.Shared.Addressing
{
    /// <summary>
    /// An object that is a part of an <see cref="AddressBookEntry"/>.
    /// </summary>
    public class EntryObject
    {
        #region Properties

        /// <summary>
        /// Gets/sets the identifier of the object.
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// Gets/sets the data object that is represented by this instance.
        /// </summary>
        public object Data { get; set; }

        #endregion
    }
}
