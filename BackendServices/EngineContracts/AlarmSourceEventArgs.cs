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
using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.EngineContracts
{
    /// <summary>
    /// Event arguments for <see cref="IAlarmSource"/>.
    /// </summary>
    public sealed class AlarmSourceEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="Operation"/>-instance that resulted from the new alarm.
        /// </summary>
        public Operation Operation { get; private set; }
        /// <summary>
        /// Gets/sets the parameters for this event args.
        /// Used within the context and contains data for jobs.
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmSourceEventArgs"/> class.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/>-instance that resulted from the new alarm.</param>
        public AlarmSourceEventArgs(Operation operation)
            : base()
        {
            this.Operation = operation;
            this.Parameters = new Dictionary<string, object>();
        }

        #endregion
    }
}