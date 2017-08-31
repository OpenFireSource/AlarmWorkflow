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

using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Windows.UIWidgets.Operation
{
    static class SettingKeys
    {
        internal const string Identifier = "OperationWidget";

        internal static readonly SettingKey LineOne = SettingKey.Create(Identifier, "LineOne");
        internal static readonly SettingKey LineTwo = SettingKey.Create(Identifier, "LineTwo");
        internal static readonly SettingKey LineThree = SettingKey.Create(Identifier, "LineThree");

        internal static readonly SettingKey SizeOne = SettingKey.Create(Identifier, "SizeOne");
        internal static readonly SettingKey SizeTwo = SettingKey.Create(Identifier, "SizeTwo");
        internal static readonly SettingKey SizeThree = SettingKey.Create(Identifier, "SizeThree");
    }
}
