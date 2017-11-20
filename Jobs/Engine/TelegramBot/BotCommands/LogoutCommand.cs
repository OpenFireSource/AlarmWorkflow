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

using AlarmWorkflow.Shared.Core;
using Telegram.Bot.Args;

namespace AlarmWorkflow.Job.TelegramBotJob.BotCommands
{
    /// <summary>
    /// BotCommand that users can logout
    /// </summary>
    [Export("LogoutCommand", typeof(IBotCommand))]
    [Information(DisplayName = "ExportLogoutCommandDisplayName", Description = "ExportLogoutCommandDescription")]
    sealed class LogoutCommand : BotCommand
    {
        #region Properties

        public override string Command => "/logout";

        public override string Usage => "/logout";
        public override string Description => "Befehl zum Abmelden vom Bot.";

        #endregion

        #region Methods

        protected override void Execute(MessageEventArgs e)
        {
            if (CurrentUser != null)
            {
                Job.Chats.Remove(CurrentUser);
                Job.SaveChats();
                Job.Bot.SendTextMessageAsync(CurrentUser.ChatId, "Du hast dich abgemeldet und erhältst keine Einsatz Benachrichtigungen mehr.");
            }
            else
            {
                Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, "Du musst dich erst anmelden um dich abmelden zu können.");
            } 
        }

        #endregion
    }
}
