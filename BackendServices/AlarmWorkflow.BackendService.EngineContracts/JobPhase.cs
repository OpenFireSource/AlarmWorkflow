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
    /// Specifies the available phases a job can run in.
    /// </summary>
    public enum JobPhase
    {
        /// <summary>
        /// Enumeration default value. Don't use.
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Represents the phase after the operation has surfaced from the alarm source and is prior to being saved.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>Jobs in this phase can provide additional information to the operation,
        /// such as loop information or other external details.</remarks>
        OnOperationSurfaced = 1,
        /// <summary>
        /// Represents the phase after the operation has been stored by the operation store.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>This is the usual phase for most jobs.</remarks>
        AfterOperationStored = 2,
    }
}