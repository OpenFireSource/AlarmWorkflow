using System;
using System.Linq;
using AlarmWorkflow.Shared.Core;
using Telegram.Bot.Args;

namespace AlarmWorkflow.Job.TelegramBotJob.BotCommands
{
    /// <summary>
    /// BotCommand that users can login with the userkey
    /// </summary>
    [Export("LoginCommand", typeof(IBotCommand))]
    [Information(DisplayName = "ExportLoginCommandDisplayName", Description = "ExportLoginCommandDescription")]
    sealed class LoginCommand : BotCommand
    {
        #region Properties

        public override string Command => "/login";

        public override string Usage => "/login [Benutzer Passwort]\r\n" +
                                        "Admin: /login [Benutzer Passwort] [Admin Passwort]";
        public override string Description => "Befehl zum Anmelden am Bot.";

        #endregion

        #region Methods

        protected override void Execute(MessageEventArgs e)
        {
            string[] fragments = e.Message.Text.Split(' ');
            if (Job.Chats.Any(x => x.Enabled && x.ChatId == e.Message.Chat.Id))
                Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, "Du bist schon angemeldet!");
            else if (!(fragments.Length == 2 || fragments.Length == 3))
                Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, "Falsche Benutzung des Befehls \"/login\"\r\n\r\n" +
                                                                "Zum Anmelden muss folgender Befehl gesendet werden: \"/login [Passwort]\"\r\n\r\n" +
                                                                "Zum Anmelden muss folgender Befehl gesendet werden: \"/login [Passwort] [AdministratorPasswort]\"");
            else if (fragments.Length == 2 && fragments[1] == Job.UserKey)
            {
                Job.Chats.Add(new Chat { Admin = false, Enabled = false, ChatId = e.Message.Chat.Id });
                Job.SaveChats();

                foreach (Chat chat in Job.Chats.Where(x => x.Admin))
                {
                    Console.WriteLine(e.Message.Chat.Id);
                    Job.Bot.SendTextMessageAsync(chat.ChatId, (string.IsNullOrWhiteSpace(e.Message.Chat.Title) ? "Der Benutzer \""+ e.Message.Chat.FirstName + " " + e.Message.Chat.LastName : "Die Gruppe \"" + e.Message.Chat.Title) + "\" hat sich angemeldet. Soll dieser/diese aktiviert werden?\r\n\r\n" +
                                                              $"/accept{e.Message.Chat.Id.ToString("D").Replace("-", "_")}\r\n\r\n" +
                                                              $"/remove{e.Message.Chat.Id.ToString("D").Replace("-", "_")}");
                }
                Job.Bot.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);

                Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, "Du bist jetzt registriert, ein Admin muss dein Konto noch bestätigen.");
            }
            else if (fragments.Length == 3 && fragments[1] == Job.UserKey && fragments[2] == Job.AdminKey)
            {
                Job.Chats.Add(new Chat { Admin = true, Enabled = true, ChatId = e.Message.Chat.Id });
                Job.SaveChats();
                Job.Bot.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);

                Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, "Du bist jetzt als Admin registriert.");
            }
            else
                Job.Bot.SendTextMessageAsync(e.Message.Chat.Id, "Autentifizierung fehlgeschlagen!");
        }

        #endregion
    }
}
