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

using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Telegram.Bot;

namespace AlarmWorkflow.Job.TelegramBotJob
{
    /// <summary>
    /// Implements a Job, that can interact with telegram users
    /// </summary>
    [Export(nameof(TelegramBotJob), typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    sealed class TelegramBotJob : IJob
    {
        #region Fields and Properties

        private List<IBotCommand> _botCommands;

        internal TelegramBotClient Bot { get; private set; }
        internal ISettingsServiceInternal Settings { get; private set; }
        internal List<Chat> Chats { get; private set; }
        internal string UserKey { get; private set; }
        internal string AdminKey { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotJob"/> class.
        /// </summary>
        public TelegramBotJob() { }

        #endregion

        #region Methods

        internal void SaveChats()
        {
            lock (Chats)
            {
                var settingItem = Settings.GetSetting(SettingKeys.ChatIds);
                settingItem.Value = JsonConvert.SerializeObject(Chats.ToArray());
                Settings.SetSetting(SettingKeys.ChatIds.Identifier, SettingKeys.ChatIds.Name, settingItem);
            }
        }

        #endregion

        #region Bot Methods

        private void _bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (!_botCommands.Any(x =>
                {
                    try
                    {
                        return x.Run(e);
                    }
                    catch (Exception exception)
                    {
                        Logger.Instance.LogException(this, exception);
                        Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.MessageHandlingFailed);
                    }
                    return false;
                })
                && e.Message.Text != null && e.Message.Text.StartsWith("/"))
            {
                _botCommands = _botCommands.OrderBy(x => x.Command).ToList();
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Es gibt folgende Befehle:");
                foreach (IBotCommand command in _botCommands)
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, command.Usage + "\r\n\r\n" + command.Description);
            }
        }

        private void _bot_OnReceiveError(object sender, Telegram.Bot.Args.ReceiveErrorEventArgs e)
        {
            Logger.Instance.LogException(this, e.ApiRequestException);
            Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.BotReceiveException);
        }

        private void Bot_OnReceiveGeneralError(object sender, Telegram.Bot.Args.ReceiveGeneralErrorEventArgs e)
        {
            Logger.Instance.LogException(this, e.Exception);
            Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.BotReceiveException);
        }

        private void Bot_OnMessageEdited(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            //Ignore Updates
        }


        #endregion

        #region IJob Members

        bool IJob.Initialize(IServiceProvider serviceProvider)
        {
            Settings = serviceProvider.GetService<ISettingsServiceInternal>();

            return InitializeChats() & InitializeBot();
        }

        private bool InitializeBot()
        {
            string apiKey = Settings.GetSetting(SettingKeys.ApiKey).GetValue<string>();
            UserKey = Settings.GetSetting(SettingKeys.UserKey).GetValue<string>();
            AdminKey = Settings.GetSetting(SettingKeys.AdminKey).GetValue<string>();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.ApiKeyEmptyWarningMessage);
                return false;
            }

            Bot = new TelegramBotClient(apiKey, new WebProxy());

            Bot.OnMessage += _bot_OnMessage;
            Bot.OnReceiveError += _bot_OnReceiveError;
            Bot.OnReceiveGeneralError += Bot_OnReceiveGeneralError;
            Bot.OnMessageEdited += Bot_OnMessageEdited;

            _botCommands = ExportedTypeLibrary.ImportAll<IBotCommand>();
            _botCommands.ForEach(x => x.Initialize(this));

            Bot.StartReceiving();
            return true;
        }

        private bool InitializeChats()
        {
            try
            {
                string chatsRaw = Settings.GetSetting(SettingKeys.ChatIds).GetValue<string>();
                if (string.IsNullOrWhiteSpace(chatsRaw))
                {
                    Chats = new List<Chat>();
                    return true;
                }
                Chat[] chatsArray = JsonConvert.DeserializeObject<Chat[]>(chatsRaw);

                Chats = new List<Chat>(chatsArray);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.ChatListInitializationErrorMessage);

                Chats = new List<Chat>();
                return false;
            }
            return true;
        }

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase != JobPhase.AfterOperationStored)
                return;

            string format = Settings.GetSetting(SettingKeys.MessageFormat).GetValue<string>();
            string text = operation.ToString(format);

            foreach (Chat chat in Chats)
            {
                Bot.SendTextMessageAsync(chat.ChatId, text);
            }
        }

        bool IJob.IsAsync => true;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Bot?.StopReceiving();
        }

        #endregion
    }
}
