﻿// This file is part of AlarmWorkflow.
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

using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("NoParser", typeof(IParser))]
    [Information(DisplayName = "ExportNoParserDisplayName", Description = "ExportNoParserDescription")]
    sealed class NoParser : IParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NoParser"/> class.
        /// Calling this constructor will log a warning stating that usage is discouraged.
        /// </summary>
        public NoParser()
        {
            Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.NoParserUsageWarning);
        }

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();

            return operation;
        }

        #endregion

    }
}
