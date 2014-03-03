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

using System.Runtime.Serialization;

namespace AlarmWorkflow.BackendService.AddressingContracts.EntryObjects
{
    /// <summary>
    /// Represents a "Loop" entry in the address book.
    /// </summary>
    [DataContract()]
    public class LoopEntryObject
    {
        #region Constants

        /// <summary>
        /// Defines the type identifier for this entry object.
        /// </summary>
        public const string TypeId = "Loop";

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the loop code represented by this entry object.
        /// </summary>
        [DataMember()]
        public string Loop { get; set; }

        #endregion
    }
}
