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

namespace AlarmWorkflow.Job.SmsJob
{
    /// <summary>
    /// Exposes the interface that can be used to extend the SMS-Job with custom providers.
    /// </summary>
    public interface ISmsProvider
    {
        /// <summary>
        /// Sends an SMS via the provider.
        /// </summary>
        /// <param name="userName">Name of the user (for login).</param>
        /// <param name="password">The password (for login).</param>
        /// <param name="phoneNumbers">The phone numbers to send the message to.</param>
        /// <param name="messageText">The message text. This text is always capped to 160 signs including ellipsis if too long.</param>
        void Send(string userName, string password, IEnumerable<string> phoneNumbers, string messageText);
    }
}