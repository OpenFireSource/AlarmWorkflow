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

using System.Linq;
using Telegram.Bot.Args;

namespace AlarmWorkflow.Job.TelegramBotJob.BotCommands
{
    /// <summary>
    /// An abstract implementation of the IBotCommand interface,
    /// with an first filter if the message applies to the by checking if the message starts with the <see cref="Command"/>.
    /// </summary>
    abstract class BotCommand : IBotCommand
    {
        #region Fields

        protected TelegramBotJob Job;
        public abstract string Command { get; }
        protected Chat CurrentUser { get; private set; }

        public abstract string Usage { get; }
        public abstract string Description { get; }

        #endregion

        #region Methods

        protected abstract void Execute(MessageEventArgs e);

        bool IBotCommand.Run(MessageEventArgs e)
        {
            if (e.Message?.Text != null && e.Message.Text.ToLower().StartsWith(Command))
            {
                CurrentUser = Job.Chats.FirstOrDefault(x => x.ChatId == e.Message.Chat.Id);
                Execute(e);
                return true;
            }
            return false;
        }

        void IBotCommand.Initialize(TelegramBotJob job)
        {
            Job = job;
        }

        #endregion
    }
}
