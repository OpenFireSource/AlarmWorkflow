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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.BackendService.Settings
{
    sealed class SettingsCollection
    {
        #region Constants

        private const string SettingsConfigurationEmbeddedResourceFileName = "settings.xml";
        private const string EmbeddedResourceFileName = "settings.info.xml";

        #endregion

        #region Fields

        private Dictionary<string, SettingsConfigurationFile> _settings;
        private SettingsDisplayConfiguration _displayConfiguration;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="SettingsDisplayConfiguration"/>.
        /// </summary>
        /// <returns></returns>
        public SettingsDisplayConfiguration SettingsDisplayConfiguration
        {
            get { return _displayConfiguration; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsCollection"/> class.
        /// </summary>
        public SettingsCollection()
        {
            _settings = new Dictionary<string, SettingsConfigurationFile>();
            Initialize();
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            List<string> assemblyFiles = new List<string>();
            assemblyFiles.AddRange(Directory.GetFiles(Utilities.GetWorkingDirectory(), "*.dll", SearchOption.TopDirectoryOnly));
            assemblyFiles.AddRange(Directory.GetFiles(Utilities.GetWorkingDirectory(), "*.exe", SearchOption.TopDirectoryOnly));

            Stopwatch sw = Stopwatch.StartNew();

            LoadSettings(assemblyFiles);

            sw.Stop();
            Logger.Instance.LogFormat(LogType.Debug, this, Properties.Resources.SettingsManagerScanSettingsFinished, sw.ElapsedMilliseconds);

            sw.Restart();
            LoadSettingsDisplayConfiguration(assemblyFiles);
            sw.Stop();
            Logger.Instance.LogFormat(LogType.Debug, this, Properties.Resources.SettingsManagerScanSettingsDisplayConfigurationFinished, sw.ElapsedMilliseconds);
        }

        private void LoadSettings(IList<string> assemblyFiles)
        {
            foreach (string assemblyFile in assemblyFiles)
            {
                string assemblyLocation = null;
                try
                {
                    Assembly assembly = Assembly.LoadFile(assemblyFile);
                    assemblyLocation = assembly.Location;

                    string embResText = assembly.GetEmbeddedResourceText(SettingsConfigurationEmbeddedResourceFileName);
                    if (string.IsNullOrWhiteSpace(embResText))
                    {
                        continue;
                    }

                    XDocument settingsConfigurationXml = XDocument.Parse(embResText);

                    SettingsConfigurationFile scf = SettingsConfigurationFileParser.Parse(settingsConfigurationXml);
                    if (scf == null)
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.SettingsConfigurationEmbResParseFailed, assemblyLocation);
                        continue;
                    }

                    _settings[scf.Identifier] = scf;
                }
                catch (XmlException ex)
                {
                    Logger.Instance.LogException(this, ex);
                    Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.SettingsConfigurationEmbResXmlException, assemblyLocation);
                }
                catch (BadImageFormatException)
                {
                    // We can ignore this exception because it may occur with unmanaged dlls.
                }
            }
        }

        // TODO: Do this in LoadSettings()!
        private void LoadSettingsDisplayConfiguration(IList<string> assemblyFiles)
        {
            SettingsDisplayConfiguration config = new SettingsDisplayConfiguration();

            foreach (string assemblyFile in assemblyFiles)
            {
                string assemblyLocation = null;
                try
                {
                    Assembly assembly = Assembly.LoadFile(assemblyFile);
                    assemblyLocation = assembly.Location;

                    string embResText = assembly.GetEmbeddedResourceText(EmbeddedResourceFileName);
                    if (string.IsNullOrWhiteSpace(embResText))
                    {
                        continue;
                    }


                    XDocument doc = XDocument.Parse(embResText);
                    ParseDisplayConfigAndAdd(doc, config);
                }
                catch (XmlException ex)
                {
                    Logger.Instance.LogException(this, ex);
                    Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.SettingsDisplayConfigurationEmbResXmlException, assemblyLocation);
                }
                catch (BadImageFormatException)
                {
                    // We can ignore this exception because it may occur with unmanaged dlls.
                }
                catch (Exception)
                {
                    // Ignore parsing this file.
                    Logger.Instance.LogFormat(LogType.Warning, null, "Could not parse settings-info file '{0}'. It either is no settings-info-file or it is in an invalid format.", assemblyLocation);
                }
            }

            _displayConfiguration = config;
        }

        private void ParseDisplayConfigAndAdd(XDocument doc, SettingsDisplayConfiguration config)
        {
            if (!doc.IsXmlValid(Properties.Resources.SettingsInfoSchema))
            {
                return;
            }

            foreach (XElement identifierE in doc.Root.Elements("Identifier"))
            {
                IdentifierInfo identifier = new IdentifierInfo();
                identifier.Name = identifierE.Attribute("Name").Value;

                identifier.DisplayText = identifierE.TryGetAttributeValue("DisplayText", identifier.Name);
                identifier.Description = identifierE.TryGetAttributeValue("Description", null);
                identifier.Order = identifierE.TryGetAttributeValue("Order", 0);
                identifier.Parent = identifierE.TryGetAttributeValue("Parent", null);

                foreach (XElement settingE in identifierE.Elements("Setting"))
                {
                    SettingInfo setting = new SettingInfo();
                    setting.Identifier = identifier.Name;
                    setting.Name = settingE.Attribute("Name").Value;

                    setting.Category = settingE.TryGetAttributeValue("Category", null);
                    setting.DisplayText = settingE.TryGetAttributeValue("DisplayText", setting.Name);
                    setting.Description = settingE.TryGetAttributeValue("Description", null);
                    setting.Order = settingE.TryGetAttributeValue("Order", 0);
                    setting.Editor = settingE.TryGetAttributeValue("Editor", null);
                    setting.EditorParameter = settingE.TryGetAttributeValue("EditorParameter", null);
                    setting.IsDynamic = settingE.TryGetAttributeValue("IsDynamic", false);

                    identifier.Settings.Add(setting);
                }

                config.Identifiers.Add(identifier);
            }
        }

        /// <summary>
        /// Returns a specific setting by its parental identifier and name.
        /// </summary>
        /// <param name="identifier">The identifier of the setting. This is used to distinguish between the different setting configurations available.</param>
        /// <param name="settingName">The name of the setting within the configuration defined by <paramref name="identifier"/>.</param>
        /// <returns>The setting by its name.
        /// -or- null, if there was no setting by this name within the configuration defined by <paramref name="identifier"/>.</returns>
        /// <exception cref="SettingIdentifierNotFoundException">A setting identifier by the name of <paramref name="identifier"/> has not been found.</exception>
        /// <exception cref="SettingNotFoundException">A setting with the name <paramref name="settingName"/> has not been found.</exception>
        public SettingItem GetSetting(string identifier, string settingName)
        {
            if (!_settings.ContainsKey(identifier))
            {
                throw new SettingIdentifierNotFoundException(identifier);
            }

            SettingItem settingItem = null;
            if (_settings.ContainsKey(identifier))
            {
                settingItem = _settings[identifier].GetSetting(settingName);
            }
            if (settingItem == null)
            {
                throw new SettingNotFoundException(settingName);
            }
            return settingItem;
        }

        #endregion
    }
}