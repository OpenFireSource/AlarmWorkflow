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
using System.Runtime.Serialization;

namespace AlarmWorkflow.BackendService.DispositioningContracts
{
    /// <summary>
    /// Contains data about a dispositioning event.
    /// </summary>
    [DataContract]
    public class DispositionEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the ID of the operation affected.
        /// </summary>
        [DataMember]
        public int OperationId { get; private set; }
        /// <summary>
        /// Gets the ID of the EMK resource to dispatch or recall.
        /// </summary>
        [DataMember]
        public string EmkResourceId { get; private set; }
        /// <summary>
        /// Gets the type of the action (dispatch/recall) that is executed.
        /// </summary>
        [DataMember]
        public ActionType Action { get; private set; }

        #endregion

        #region Constructors

        private DispositionEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispositionEventArgs"/> class.
        /// </summary>
        /// <param name="operationId">The ID of the operation affected.</param>
        /// <param name="emkResourceId">The ID of the EMK resource to dispatch or recall.</param>
        /// <param name="action">The type of the action (dispatch/recall) that is executed.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">An invalid value was present for <paramref name="action"/>.</exception>
        public DispositionEventArgs(int operationId, string emkResourceId, ActionType action)
            : this()
        {
            if (action == ActionType.Invalid)
            {
                throw new ArgumentOutOfRangeException("action");
            }

            this.OperationId = operationId;
            this.EmkResourceId = emkResourceId;
            this.Action = action;
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Specifies the type of the action (dispatch or recall).
        /// </summary>
        public enum ActionType
        {
            /// <summary>
            /// Enumeration default value. Using this will cause exceptions.
            /// </summary>
            Invalid = 0,
            /// <summary>
            /// Dispatching (or, "dispositioning") a resource.
            /// </summary>
            Dispatch,
            /// <summary>
            /// Recalling (or, "un-dispositioning") a resource.
            /// </summary>
            Recall,
        }

        #endregion
    }
}
