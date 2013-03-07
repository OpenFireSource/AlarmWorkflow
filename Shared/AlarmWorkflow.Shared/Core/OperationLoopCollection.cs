using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Represents a strongly-typed collection that holds loop information from an Operation.
    /// </summary>
    [Serializable()]
    public sealed class OperationLoopCollection : Collection<string>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationLoopCollection"/> class.
        /// </summary>
        public OperationLoopCollection()
            : base()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Overridden to avoid adding the same loop if already existing. It also doesn't add an item if it is null or empty.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, string item)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                return;
            }

            item = item.Trim();

            if (ContainsItem(item))
            {
                return;
            }

            base.InsertItem(index, item);
        }

        private bool ContainsItem(string item)
        {
            return this.Items.Any(i => i.Equals(item, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Overridden to avoid adding the same loop if already existing. It also doesn't add an item if it is null or empty.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, string item)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                return;
            }

            item = item.Trim();

            if (ContainsItem(item))
            {
                return;
            }

            base.SetItem(index, item);
        }

        #endregion
    }
}
