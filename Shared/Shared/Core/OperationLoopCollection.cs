// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

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
        #region Constants

        /// <summary>
        /// Defines the separator used when joining loops together to a string.
        /// </summary>
        public static readonly char LoopSeparator = ';';

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationLoopCollection"/> class.
        /// </summary>
        public OperationLoopCollection()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationLoopCollection"/> class,
        /// and adds all loops in the given, separated list to this collection.
        /// </summary>
        /// <param name="loops">A string representing a separated list of loops, that may have been created by <see cref="ToString()"/>.</param>
        public OperationLoopCollection(string loops)
            :this()
        {
            if (!string.IsNullOrWhiteSpace(loops))
            {
                foreach (string item in loops.Split(LoopSeparator))
                {
                    this.Add(item);
                }
            }
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

        /// <summary>
        /// Returns all loop items, joined together to a string.
        /// </summary>
        /// <returns>All loop items joined together to a string.</returns>
        public override string ToString()
        {
            return string.Join(LoopSeparator.ToString(), this.Items);
        }

        #endregion
    }
}