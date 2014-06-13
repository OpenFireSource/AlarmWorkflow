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


namespace AlarmWorkflow.Windows.UIContracts.ViewModels
{
    /// <summary>
    /// Specifies the states in which a task inside a <see cref="BackgroundTaskCommand"/> can be.
    /// </summary>
    public enum BackgroundTaskState
    {
        /// <summary>
        /// The task has not been started yet.
        /// </summary>
        Unstarted = 0,
        /// <summary>
        /// The task is currently running.
        /// </summary>
        Running,
        /// <summary>
        /// The task has successfully completed.
        /// </summary>
        Completed,
        /// <summary>
        /// The task has prematurely ended due to an exception.
        /// </summary>
        Faulted,
    }
}
