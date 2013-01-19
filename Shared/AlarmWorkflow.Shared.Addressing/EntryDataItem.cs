
namespace AlarmWorkflow.Shared.Addressing
{
    /// <summary>
    /// An object that is a part of an <see cref="AddressBookEntry"/>.
    /// </summary>
    public class EntryDataItem
    {
        #region Properties

        /// <summary>
        /// Gets/sets the identifier of the object.
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// Gets/sets whether this entry data item is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// Gets/sets the data object that is represented by this instance.
        /// </summary>
        public object Data { get; set; }

        #endregion
    }
}
