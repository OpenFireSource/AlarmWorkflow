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
    /// Represents a "Push" entry in the address book. 
    /// See documentation for further information.
    /// </summary>
    /// <remarks>This is a generalized way to talk any push-notification-consumer, including (but not limited to): Prowl, Growl etc.</remarks>
    [DataContract()]
    public class PushEntryObject
    {
        #region Constants

        /// <summary>
        /// Defines the type identifier for this entry object.
        /// </summary>
        public const string TypeId = "Push";

        #endregion

        #region Fields

        /// <summary>
        /// Defines the consumers that are supported by default.
        /// </summary>
        public static readonly string[] DefaultConsumers = { "NMA", "Prowl", "eAlarm" };

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the name of the consumer of push notifications.
        /// </summary>
        [DataMember()]
        public string Consumer { get; set; }
        /// <summary>
        /// Gets/sets the API-key of the recipient.
        /// </summary>
        [DataMember()]
        public string RecipientApiKey { get; set; }

        #endregion
    }
}
