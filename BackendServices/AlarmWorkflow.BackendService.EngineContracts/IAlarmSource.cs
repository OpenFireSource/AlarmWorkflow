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

namespace AlarmWorkflow.BackendService.EngineContracts
{
    /// <summary>
    /// Defines a means for a type that acts as a source of Operations, that is a type that processes alarms, such as a Fax, SMS or E-Mail.
    /// </summary>
    public interface IAlarmSource : IDisposable
    {
        /// <summary>
        /// Raised when a new alarm has surfaced and processed for the Engine to handle.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>This event handler is only registered by the Engine if Initialization was successful.</remarks>
        event EventHandler<AlarmSourceEventArgs> NewAlarm;

        /// <summary>
        /// Initializes this alarm source prior to its first use.
        /// </summary>
        /// <param name="serviceProvider">The service provider used for retrieving server-side services.</param>
        void Initialize(IServiceProvider serviceProvider);
        /// <summary>
        /// Called by the Engine when initialization has succeeded. This method call is run within its own thread.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>The thread that contains this method is run when the Engine starts, and is aborted when the Engine stops.
        /// Exceptions that are thrown in this method are logged and handled.</remarks>
        void RunThread();
    }
}