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
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.AlarmSource.Sms
{
    [Export("DefaultSMSParser", typeof(IParser))]
    class DefaultSMSParser : IParser
    {
        #region ISmsParser Members

        Operation IParser.Parse(string[] lines)
        {
            string text = string.Join("", lines);

            Operation operation = new Operation();
            operation.Timestamp = DateTime.Now;
            operation.Comment = text;

            return operation;
        }

        #endregion
    }
}