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

namespace AlarmWorkflow.BackendService.EngineContracts
{
    /// <summary>
    /// Defines a means for a job, which is a task that runs on every new Operation.
    /// </summary>
    public interface IJob : IDisposable
    {
        /// <summary>
        /// Returns whether or not this job is executed asynchronously by the engine.
        /// This should be set to true for longer-running tasks.
        /// </summary>
        bool IsAsync { get; }

        /// <summary>
        /// Called to initialize the job when it gets added to the job queue that runs on each processed alarm.
        /// </summary>
        /// <param name="serviceProvider">The service provider used for retrieving server-side services.</param>
        /// <returns>Whether or not initialization was successful.</returns>
        bool Initialize(IServiceProvider serviceProvider);
        /// <summary>
        /// Executes the job for the given context and operation.
        /// </summary>
        /// <param name="context">The context in which the current job runs.</param>
        /// <param name="operation">Current operation.</param>
        void Execute(IJobContext context, Operation operation);
    }
}