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
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.UIContracts.Extensibility
{
    /// <summary>
    /// Defines a means for a job that runs on UI-side after a new <see cref="Operation"/> has come in.
    /// This is the UI-side equivalent to the "IJob"-interface of the AlarmWorkflowEngine.
    /// </summary>
    public interface IUIJob : IDisposable
    {
        /// <summary>
        /// Returns whether or not this job is executed asynchronously.
        /// This should be set to true for longer-running tasks.
        /// </summary>
        bool IsAsync { get; }

        /// <summary>
        /// Initializes this job prior to its first use.
        /// </summary>
        /// <returns>The result of the initialization. Jobs that return false won't be called.</returns>
        bool Initialize();
        /// <summary>
        /// Runs the job-logic on the new operation.
        /// </summary>
        /// <param name="operationViewer">The instance of the <see cref="IOperationViewer"/> on which this job runs.</param>
        /// <param name="operation">The operation that has come in.</param>
        void OnNewOperation(IOperationViewer operationViewer, Operation operation);
    }
}