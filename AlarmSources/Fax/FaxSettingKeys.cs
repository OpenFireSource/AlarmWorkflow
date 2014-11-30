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

namespace AlarmWorkflow.AlarmSource.Fax
{
    static class FaxSettingKeys
    {
        private const string Identifier = "FaxAlarmSource";

        internal static readonly SettingKey FaxBlacklist = SettingKey.Create(Identifier, "FaxBlacklist");
        internal static readonly SettingKey FaxWhitelist = SettingKey.Create(Identifier, "FaxWhitelist");
        internal static readonly SettingKey FaxPath = SettingKey.Create(Identifier, "FaxPath");
        internal static readonly SettingKey ArchivePath = SettingKey.Create(Identifier, "ArchivePath");
        internal static readonly SettingKey AnalysisPath = SettingKey.Create(Identifier, "AnalysisPath");
        internal static readonly SettingKey AlarmFaxParserAlias = SettingKey.Create(Identifier, "AlarmfaxParser");
        internal static readonly SettingKey OcrPath = SettingKey.Create(Identifier, "OCR.Path");
    }
}
