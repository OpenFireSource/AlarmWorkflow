using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlarmWorkflow.Shared.Core;
using Telegram.Bot.Args;

namespace AlarmWorkflow.Job.TelegramBotJob.BotCommands
{
    /// <summary>
    /// BotCommand that list all users
    /// </summary>
    [Export("ListUsersCommand", typeof(IBotCommand))]
    [Information(DisplayName = "ExportListUsersCommandDisplayName", Description = "ExportListUsersCommandDescription")]
    sealed class ListUsersCommand : BotCommand
    {
        #region Properties

        public override string Command => "/listusers";
        public override string Usage => "Admin: /listusers";
        public override string Description => "Listet alle Benutzer auf";

        #endregion

        #region Methods


        protected override void Execute(MessageEventArgs e)
        {
            if (CurrentUser == null || !CurrentUser.Admin)
            {
                Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, "Zugriff verweigert!");
            }
            else
            {
                foreach (Chat chat in Job.Chats)
                {
                    var chatData = Job.Bot.GetChatAsync(chat.ChatId).Result;
                    Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, (string.IsNullOrWhiteSpace(chatData.Title) ? chatData.FirstName + " " + chatData.LastName: chatData.Title) +"\r\n" +
                                                                    (chat.Admin ? "Admin; " : "") + (chat.Enabled ? "Aktiv" : "Inaktiv") + "\r\n" +
                                                                    $"/remove{chat.ChatId.ToString("D").Replace("-", "_")}");
                }
            }
        }

        #endregion
    }
}
