﻿// This file is part of AlarmWorkflow.
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

using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Job.TelegramBotJob
{
    static class SettingKeys
    {
        internal static readonly SettingKey ApiKey = SettingKey.Create("TelegramBotJob", "ApiKey");
        internal static readonly SettingKey UserKey = SettingKey.Create("TelegramBotJob", "UserKey");
        internal static readonly SettingKey AdminKey = SettingKey.Create("TelegramBotJob", "AdminKey");
        internal static readonly SettingKey MessageFormat = SettingKey.Create("TelegramBotJob", "MessageFormat");

        internal static readonly SettingKey ChatIds = SettingKey.Create("TelegramBotJob", "Chats");
    }
    
}
