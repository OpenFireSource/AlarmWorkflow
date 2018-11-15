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
    /// BotCommand that admins can remove users
    /// </summary>
    [Export("RemoveCommand", typeof(IBotCommand))]
    [Information(DisplayName = "ExportRemoveCommandDisplayName", Description = "ExportRemoveCommandDescription")]
    sealed class RemoveCommand : BotCommand
    {
        #region Properties

        public override string Command => "/remove";

        public override string Usage => "Admin: /remove[ChatId]";
        public override string Description => "Befehl zum Entfernen eines Chats/Benutzers vom Bot.";

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
                    else 
                    {
                        Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, "Der Benutzer wird gesperrt.");
                        Job.Chats.Remove(chat);
                        Job.SaveChats();
                        Job.Bot.SendTextMessageAsync(chatId, "Du wurdest entfernt und erhälst jetzt keine Einsatz Benachrichtigungen mehr.");
                    }
                }
                else
                {
                    Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Falsche Benutzung des Befehls \"{Command}\".");
                }
            }
        }
        
        #endregion
    }
}
