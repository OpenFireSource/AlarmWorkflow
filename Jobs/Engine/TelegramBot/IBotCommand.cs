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

using Telegram.Bot.Args;

namespace AlarmWorkflow.Job.TelegramBotJob
{
    /// <summary>
    /// Exposes the interface that can be used to extend the TelegramBot with custom Commands.
    /// </summary>
    interface IBotCommand
    {
        string Command { get; }
        string Usage { get; }
        string Description { get; }

        /// <summary>
        /// Send the message to the Command
        /// </summary>
        /// <param name="e">the Message Details</param>
        /// <returns>return true if the command has done something with the message or not.</returns>
        bool Run(MessageEventArgs e);

        /// <summary>
        /// Initialize the Command.
        /// </summary>
        /// <param name="job"></param>
        void Initialize(TelegramBotJob job);
    }
}
