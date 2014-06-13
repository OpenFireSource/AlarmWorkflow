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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.BackendService.Settings
{
    /// <summary>
    /// Provides the logic to parse the various setting configuration files.
    /// </summary>
    static class SettingsConfigurationFileParser
    {
        /// <summary>
        /// Parses the settings configuration XML-document according to its version.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        internal static SettingsConfigurationFile Parse(XDocument document)
        {
            if (!document.IsXmlValid(Properties.Resources.SettingsSchema))
            {
                return null;
            }
            return ParseCore(document.Root);
        }

        private static SettingsConfigurationFile ParseCore(XElement rootE)
        {
            string identifier = rootE.Attribute("Identifier").Value;

            List<SettingItem> settings = new List<SettingItem>();
            foreach (XElement settingE in rootE.Elements("Setting"))
            {
                string name = settingE.Attribute("Name").Value;
                string typeName = settingE.Attribute("Type").Value;
                bool isNull = settingE.TryGetAttributeValue("IsNull", false);

                // Read the setting value. If it contains a CDATA, then we need to process that first.
                string valueString = null;
                XNode valueNode = settingE.DescendantNodes().FirstOrDefault();
                if (valueNode != null)
                {
                    switch (valueNode.NodeType)
                    {
                        case System.Xml.XmlNodeType.CDATA:
                        case System.Xml.XmlNodeType.Text:
                            valueString = ((XText)valueNode).Value;
                            break;
                        default:
                            Logger.Instance.LogFormat(LogType.Warning, typeof(SettingsConfigurationFile), Properties.Resources.SettingsConfigurationEmbResInvalidValueContent, valueNode.NodeType, name);
                            break;
                    }
                }

                Type type = Type.GetType(typeName);

                SettingItem settingItem = new SettingItem(identifier, name, valueString, type);
                settings.Add(settingItem);
            }

            return new SettingsConfigurationFile(identifier, settings);
        }
    }
}