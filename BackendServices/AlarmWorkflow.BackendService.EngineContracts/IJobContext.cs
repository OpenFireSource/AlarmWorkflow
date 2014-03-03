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

using System.Collections.Generic;

namespace AlarmWorkflow.BackendService.EngineContracts
{
    /// <summary>
    /// Defines an interface for a context, which is created by the engine and handed over to plugins.
    /// </summary>
    public interface IJobContext
    {
        /// <summary>
        /// Gets the name of the alarm source that this context runs in.
        /// </summary>
        string AlarmSourceName { get; }
        /// <summary>
        /// Gets an alarm-source specific array of parameters that are associated with this context and alarm source.
        /// This property is filled by the alarm source.
        /// </summary>
        IDictionary<string, object> Parameters { get; }
        /// <summary>
        /// Gets the phase in which this job is executed.
        /// </summary>
        JobPhase Phase { get; }
    }
}