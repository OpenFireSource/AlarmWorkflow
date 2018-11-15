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
using AlarmWorkflow.Shared.Core;
using Telegram.Bot.Args;

namespace AlarmWorkflow.Job.TelegramBotJob.BotCommands
{
    /// <summary>
    /// BotCommand that admins can activate users
    /// </summary>
    [Export("AcceptCommand", typeof(IBotCommand))]
    [Information(DisplayName = "ExportAcceptCommandDisplayName", Description = "ExportAcceptCommandDescription")]
    sealed class AcceptCommand : BotCommand
    {
        #region Properties

        public override string Command => "/accept";

        public override string Usage => "Admin: /accept[ChatId]";
        public override string Description => "Befehl zum Aktivieren eines Chats/Benutzers vom Bot.";

        #endregion

        #region Methods

        protected override void Execute(MessageEventArgs e)
        {
            if (CurrentUser == null || !CurrentUser.Admin)
            {
                Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, "Zugriff verweigert!");
            }
            else if (e.Message.Text == Command)
            {
                Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, "Falsche Benutzung des Befehls \"/accept\".");
            }
            else
            {
                string msgData = e.Message.Text.Replace(Command, "").Replace("_", "-");
                long chatId;
                if (long.TryParse(msgData, out chatId))
                {
                    Chat chat = Job.Chats.FirstOrDefault(x => x.ChatId == chatId);
                    if (chat == null)
                    {
                        Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, "Es existiert kein Benutzer mit dieser Id.");
                    }
                    else if (chat.Enabled)
                    {
                        Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, "Der Benutzer wurde schon freigegeben.");
                    }
                    else
                    {
                        Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, "Der Benutzer wird freigegeben.");
                        chat.Enabled = true;
                        Job.SaveChats();
                        Job.Bot.SendTextMessageAsync(chatId, "Du wurdest freigeschalten und erhälst jetzt alle Einsatz Benachrichtigungen.");
                    }
                }
                else
                {
                    Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, "Falsche Benutzung des Befehls \"/accept\".");
                }
            }
        }

        #endregion
    }
}
