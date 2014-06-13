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
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.BackendService.EngineContracts;

namespace AlarmWorkflow.BackendService.Engine
{
    class JobContext : IJobContext
    {
        #region Constructors

        internal JobContext(IAlarmSource source, AlarmSourceEventArgs args)
        {
            Assertions.AssertNotNull(source, "source");
            Assertions.AssertNotNull(args, "args");

            AlarmSourceName = source.GetType().Name;
            Parameters = args.Parameters;
        }

        #endregion

        #region IJobContext Members

        /// <summary>
        /// Gets the name of the alarm source that this context runs in.
        /// </summary>
        public string AlarmSourceName { get; private set; }

        /// <summary>
        /// Gets an alarm-source specific array of parameters that are associated with this context and alarm source.
        /// This property is filled by the alarm source.
        /// </summary>
        public IDictionary<string, object> Parameters { get; private set; }

        /// <summary>
        /// Gets the phase in which this job is executed.
        /// </summary>
        public JobPhase Phase { get; internal set; }

        #endregion
    }
}