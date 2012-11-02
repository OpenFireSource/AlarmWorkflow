using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Windows.Configuration.Config
{
    /// <summary>
    /// Represents the "SettingsDisplayConfiguration.xml" file.
    /// </summary>
    class SettingsDisplayConfiguration
    {
        #region Constants

        private static readonly string InfoDirectory = Path.Combine(Utilities.GetWorkingDirectory(), "Config", "Info");

        #endregion

        #region Properties

        /// <summary>
        /// Gets a collection of all registered identifiers.
        /// </summary>
        public List<IdentifierInfo> Identifiers { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="SettingsDisplayConfiguration"/> class from being created.
        /// </summary>
        private SettingsDisplayConfiguration()
        {
            Identifiers = new List<IdentifierInfo>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Scans the "Config/Info" directory and accumulates all info-xml files and returns one
        /// SettingsDisplayConfiguration-object that contains the info of them all.
        /// </summary>
        /// <returns></returns>
        public static SettingsDisplayConfiguration Load()
        {
            SettingsDisplayConfiguration config = new SettingsDisplayConfiguration();

            // Find all info-files
            DirectoryInfo dir = new DirectoryInfo(InfoDirectory);
            foreach (FileInfo file in dir.GetFiles("*.xml", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    XDocument doc = XDocument.Load(file.FullName);
                    if (doc.Root.Name.LocalName != "SettingsDisplayConfiguration")
                    {
                        continue;
                    }

                    // Iterate over all <Identifier />-tags
                    foreach (XElement identifierE in doc.Root.Elements("Identifier"))
                    {
                        // Read information from attribs
                        IdentifierInfo identifier = new IdentifierInfo();
                        identifier.Name = identifierE.TryGetAttributeValue("Name", null);
                        if (string.IsNullOrWhiteSpace(identifier.Name))
                        {
                            // TODO: Log warning
                            continue;
                        }

                        identifier.DisplayText = identifierE.TryGetAttributeValue("DisplayText", identifier.Name);
                        identifier.Description = identifierE.TryGetAttributeValue("Description", null);
                        identifier.Order = identifierE.TryGetAttributeValue("Order", 0);

                        // Iterate over all <Setting />-tags
                        foreach (XElement settingE in identifierE.Elements("Setting"))
                        {
                            SettingInfo setting = new SettingInfo();
                            setting.Name = settingE.TryGetAttributeValue("Name", null);
                            if (string.IsNullOrWhiteSpace(setting.Name))
                            {
                                // TODO: Log warning
                                continue;
                            }

                            setting.Category = settingE.TryGetAttributeValue("Category", null);
                            setting.DisplayText = settingE.TryGetAttributeValue("DisplayText", setting.Name);
                            setting.Description = settingE.TryGetAttributeValue("Description", null);
                            setting.Order = settingE.TryGetAttributeValue("Order", 0);
                            setting.Editor = settingE.TryGetAttributeValue("Editor", null);
                            setting.EditorParameter = settingE.TryGetAttributeValue("EditorParameter", null);

                            identifier.Settings.Add(setting);
                        }

                        config.Identifiers.Add(identifier);
                    }
                }
                catch (Exception)
                {
                    // Ignore parsing this file.
                    Logger.Instance.LogFormat(LogType.Warning, null, "Could not parse settings-info file '{0}'. It either is no settings-info-file or it is in an invalid format.", file.FullName);
                }
            }

            return config;
        }

        /// <summary>
        /// Tries to find the <see cref="IdentifierInfo"/> for the section by the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IdentifierInfo GetIdentifier(string name)
        {
            return Identifiers.Find(i => i.Name == name);
        }

        /// <summary>
        /// Tries to find the <see cref="SettingInfo"/> for the setting within the given section and name.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public SettingInfo GetSetting(string identifier, string name)
        {
            IdentifierInfo inf = GetIdentifier(identifier);
            if (inf != null)
            {
                return inf.Settings.Find(s => s.Name == name);
            }
            return null;
        }

        #endregion
    }
}
