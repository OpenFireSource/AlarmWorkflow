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
using System.Linq;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.BackendService.Engine
{
    class AlarmFilter : IAlarmFilter
    {
        #region Properties

        internal Configuration Configuration { get; set; }

        #endregion

        #region IAlarmFilter Members

        bool IAlarmFilter.QueryAcceptSource(string source)
        {
            if (source == null)
            {
                return false;
            }

            /* FIXME: Actually, it's not very clean code to have this method (which only performs evaluation) output log messages,
             * since it's not actually using the result by itself (the user of this interface does).
             * However, it appears as this is somewhat efficient as opposed to replicating the messages every time they are used.
             */

            if (!IsOnWhitelist(source))
            {
                Logger.Instance.LogFormat(LogType.Info, this, Properties.Resources.SourceIsNotOnWhitelist);
                return false;
            }

            if (IsOnBlacklist(source))
            {
                Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.SourceIsOnBlacklist);
                return false;
            }

            return true;
        }

        private bool IsOnBlacklist(string source)
        {
            return Configuration.GlobalBlacklist.Any(kw => source.Contains(kw));
        }

        private bool IsOnWhitelist(string source)
        {
            IEnumerable<string> whitelist = Configuration.GlobalWhitelist;
            if (!whitelist.Any())
            {
                return true;
            }

            return whitelist.Any(kw => source.Contains(kw));
        }

        #endregion
    }
}
