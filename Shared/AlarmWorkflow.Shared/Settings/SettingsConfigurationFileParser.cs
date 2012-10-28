using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using System.Globalization;

namespace AlarmWorkflow.Shared.Settings
{
    /// <summary>
    /// Provides the logic to parse the various setting configuration files.
    /// </summary>
    static class SettingsConfigurationFileParser
    {
        private static readonly string[] SupportedSettingTypes = new[] { "System.String", "System.Int32", "System.Boolean", "System.Single", "System.Double" };

        /// <summary>
        /// Parses the settings configuration XML-document according to its version.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        internal static SettingsConfigurationFile Parse(XDocument document)
        {
            int version = document.Root.TryGetAttributeValue("Version", 1);
            switch (version)
            {
                case 1:
                default:
                    return ParseVersion1(document.Root);
            }
        }

        private static SettingsConfigurationFile ParseVersion1(XElement rootE)
        {
            string identifier = rootE.TryGetAttributeValue("Identifier", null);

            if (string.IsNullOrWhiteSpace(identifier))
            {
                return null;
            }

            int iSetting = 0;

            List<SettingItem> settings = new List<SettingItem>();
            foreach (XElement settingE in rootE.Elements("Setting"))
            {
                // Dissect the element and retrieve all attributes
                string name = settingE.TryGetAttributeValue("Name", null);
                if (string.IsNullOrWhiteSpace(name))
                {
                    Logger.Instance.LogFormat(LogType.Warning, typeof(SettingsConfigurationFileParser), Properties.Resources.SettingItemInvalidName, iSetting + 1);
                    continue;
                }

                string typeName = settingE.TryGetAttributeValue("Type", null);
                if (string.IsNullOrWhiteSpace(typeName))
                {
                    Logger.Instance.LogFormat(LogType.Warning, typeof(SettingsConfigurationFileParser), Properties.Resources.SettingItemEmptyType, name);
                    continue;
                }
                if (!SupportedSettingTypes.Contains(typeName))
                {
                    Logger.Instance.LogFormat(LogType.Warning, typeof(SettingsConfigurationFileParser), Properties.Resources.SettingItemInvalidType, typeName, string.Join(",", SupportedSettingTypes));
                    continue;
                }

                bool isNull = settingE.TryGetAttributeValue("IsNull", false);
                string editorName = settingE.TryGetAttributeValue("Editor", null);

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

                // TODO: This will work only with primitive types (String, Boolean etc.). This could be made extensible to allow storing other types as well.
                Type type = Type.GetType(typeName);
                object defaultValue = Convert.ChangeType(valueString, type, CultureInfo.InvariantCulture);

                SettingItem settingItem = new SettingItem(name, defaultValue, defaultValue, type, editorName);
                settings.Add(settingItem);

                iSetting++;
            }


            return new SettingsConfigurationFile(identifier, settings);
        }
    }
}
