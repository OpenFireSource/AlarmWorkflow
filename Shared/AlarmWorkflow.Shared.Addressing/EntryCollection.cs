using System;
using System.Collections.ObjectModel;

namespace AlarmWorkflow.Shared.Addressing
{
    /// <summary>
    /// Represents a strongly-typed collection that manages <see cref="AddressBookEntry"/>-items.
    /// </summary>
    public sealed class EntryCollection : Collection<AddressBookEntry>
    {
        #region Methods
        
        /// <summary>
        /// Overridden to avoid adding double entries.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, AddressBookEntry item)
        {
            if (this.Items.Contains(item))
            {
                return;
            }
            base.SetItem(index, item);
        }

        /// <summary>
        /// Overridden to avoid adding double entries.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, AddressBookEntry item)
        {
            if (this.Items.Contains(item))
            {
                return;
            }
            base.InsertItem(index, item);
        }

        #endregion
    }
}
