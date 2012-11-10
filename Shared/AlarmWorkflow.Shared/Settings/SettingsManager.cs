using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Shared.Settings
{
    /// <summary>
    /// Manages the settings in the scope of the application.
    /// </summary>
    public sealed class SettingsManager : IEnumerable<SettingDescriptor>
    {
        #region Constants

        private const string SettingsConfigurationEmbeddedResourceFileName = "settings.xml";
        private const string UserSettingsFileName = "user.settings";

        private static readonly string UserSettingsFilePath = Utilities.GetLocalAppDataFolderFileName(UserSettingsFileName);

        #endregion

        #region Singleton

        private static SettingsManager _instance;
        /// <summary>
        /// Gets the singleton Instance of this type.
        /// </summary>
        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SettingsManager();
                }
                return _instance;
            }
        }

        #endregion

        #region Fields

        private bool _isInitialized;
        private Dictionary<string, SettingsConfigurationFile> _settings;
        private SettingsDisplayConfiguration _displayConfiguration;

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="SettingsManager"/> class from being created.
        /// </summary>
        private SettingsManager()
        {
            _settings = new Dictionary<string, SettingsConfigurationFile>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes this instance and loads only the settings (no display configuration).
        /// </summary>
        public void Initialize()
        {
            Initialize(SettingsInitialization.OnlySettings);
        }

        /// <summary>
        /// Initializes this instance and loads only the settings (no display configuration).
        /// </summary>
        /// <param name="settingsInitialization"></param>
        public void Initialize(SettingsInitialization settingsInitialization)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException("Instance is already initialized!");
            }

            // Scan all assemblies within the current path and try to read their "settings.xml" file (must be named like that!).
            List<string> assemblyFiles = new List<string>();
            assemblyFiles.AddRange(Directory.GetFiles(Utilities.GetWorkingDirectory(), "*.dll", SearchOption.TopDirectoryOnly));
            assemblyFiles.AddRange(Directory.GetFiles(Utilities.GetWorkingDirectory(), "*.exe", SearchOption.TopDirectoryOnly));

            LoadSettings(assemblyFiles);
            // If we shall also initialize the display configuration
            if (settingsInitialization == SettingsInitialization.IncludeDisplayConfiguration)
            {
                LoadSettingsDisplayConfiguration(assemblyFiles);
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Invalidates all current settings (without saving them). 
        /// When this method has finished, you need to call Initialize() again.
        /// </summary>
        public void Invalidate()
        {
            _displayConfiguration = null;
            _settings.Clear();
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

                    // Try to locate and load the embedded resource file
                    string embResText = assembly.GetEmbeddedResourceText(SettingsConfigurationEmbeddedResourceFileName);
                    // If the assembly has no such settings configuration, skip further processing.
                    if (string.IsNullOrWhiteSpace(embResText))
                    {
                        continue;
                    }

                    // Try to parse the settings configuration file
                    XDocument settingsConfigurationXml = XDocument.Parse(embResText);

                    SettingsConfigurationFile scf = SettingsConfigurationFileParser.Parse(settingsConfigurationXml);
                    // Make a check if the configuration file has failed to parse
                    if (scf == null)
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.SettingsConfigurationEmbResParseFailed, assemblyLocation);
                        continue;
                    }

                    // Success. Add the file to the dictionary.
                    _settings[scf.Identifier] = scf;

                    Logger.Instance.LogFormat(LogType.Debug, this, Properties.Resources.SettingsConfigurationEmbResLoaded, assemblyLocation);
                }
                catch (XmlException ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.SettingsConfigurationEmbResXmlException, assemblyLocation, ex.Message);
                }
                catch (BadImageFormatException)
                {
                    // We can ignore this exception because it may occur with unmanaged dlls.
                }
                catch (Exception)
                {
                    throw;
                }
            }

            // After the configuration files have been parsed, we need to see if there are any user-configuration files and read those values in.
            // User values override default values.
            LoadUserConfigurationFile();
        }

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

                    // Try to locate and load the embedded resource file
                    string embResText = assembly.GetEmbeddedResourceText(SettingsDisplayConfiguration.EmbeddedResourceFileName);
                    // If the assembly has no such settings configuration, skip further processing.
                    if (string.IsNullOrWhiteSpace(embResText))
                    {
                        continue;
                    }


                    XDocument doc = XDocument.Parse(embResText);
                    config.ParseAdd(doc);

                    Logger.Instance.LogFormat(LogType.Debug, this, Properties.Resources.SettingsDisplayConfigurationEmbResLoaded, assemblyLocation);
                }
                catch (XmlException ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.SettingsDisplayConfigurationEmbResLoaded, assemblyLocation, ex.Message);
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

            // Done loading!
            _displayConfiguration = config;
        }

        /// <summary>
        /// Locates the user-configuration file (if present yet) and loads the user-values into the setting items.
        /// </summary>
        private void LoadUserConfigurationFile()
        {
            // If the file does not exist, there is nothing to do yet.
            if (!File.Exists(UserSettingsFilePath))
            {
                return;
            }

            // Read the XML-Document
            XDocument doc = null;
            using (StreamReader reader = new StreamReader(UserSettingsFilePath, Encoding.UTF8))
            {
                doc = XDocument.Load(reader);
            }

            foreach (XElement sectionE in doc.Root.Elements("Section"))
            {
                string identifier = sectionE.TryGetAttributeValue("Identifier", null);
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    // TODO: Log
                    continue;
                }

                foreach (XElement userSettingE in sectionE.Elements("UserSetting"))
                {
                    string name = userSettingE.TryGetAttributeValue("Name", null);
                    if (string.IsNullOrWhiteSpace(identifier))
                    {
                        // TODO: Log
                        continue;
                    }

                    bool isNull = userSettingE.TryGetAttributeValue("IsNull", false);
                    string value = userSettingE.Value;

                    // Try to retrieve the setting. Ignore if the setting does not exist.
                    SettingItem affectedSettingItem = GetSetting(identifier, name, false);
                    if (affectedSettingItem == null)
                    {
                        continue;
                    }
                    affectedSettingItem.SetStringValue(value, isNull, false);
                }
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
            return GetSetting(identifier, settingName, true);
        }

        private SettingItem GetSetting(string identifier, string settingName, bool throwExceptionIfMissing)
        {
            if (!_settings.ContainsKey(identifier) && throwExceptionIfMissing)
            {
                throw new SettingIdentifierNotFoundException(identifier);
            }

            SettingItem settingItem = _settings[identifier].GetSetting(settingName);
            if (settingItem == null && throwExceptionIfMissing)
            {
                throw new SettingNotFoundException(settingName);
            }
            return settingItem;
        }

        /// <summary>
        /// Saves all settings to the disk, including their changes.
        /// </summary>
        public void SaveSettings()
        {
            XDocument doc = new XDocument();

            XElement rootE = new XElement("UserSettings");
            rootE.Add(new XAttribute("Version", 1));
            doc.Add(rootE);

            // Store setting values
            foreach (var pair in _settings)
            {
                XElement identifyableE = new XElement("Section");
                identifyableE.Add(new XAttribute("Identifier", pair.Key));

                foreach (SettingItem item in pair.Value)
                {
                    XElement settingE = new XElement("UserSetting");
                    settingE.Add(new XAttribute("Name", item.Name));

                    string value = "";
                    // Special case: Check if the value is null because that can't be serialized into XML.
                    if (item.Value == null)
                    {
                        // If the setting is allowed to be null (reference types = classes) then 
                        if (SettingItem.CanBeNull(item))
                        {
                            settingE.Add(new XAttribute("IsNull", true));
                        }
                    }
                    else
                    {
                        value = Convert.ToString(item.Value, CultureInfo.InvariantCulture);
                    }

                    settingE.Value = value;

                    identifyableE.Add(settingE);
                }

                rootE.Add(identifyableE);
            }

            // Save to disk using explicit encoding
            using (StreamWriter writer = new StreamWriter(UserSettingsFilePath, false, Encoding.UTF8))
            {
                doc.Save(writer);
            }
        }

        /// <summary>
        /// Returns the <see cref="SettingsDisplayConfiguration"/>, if it was included in initialization.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">This instance was not initialized with "IncludeDisplayConfiguration" or the display configuration is not available at this point.</exception>
        public SettingsDisplayConfiguration GetSettingsDisplayConfiguration()
        {
            if (_displayConfiguration == null)
            {
                throw new InvalidOperationException(Properties.Resources.SettingsDisplayConfigurationNotFoundException);
            }
            return _displayConfiguration;
        }

        #endregion

        #region IEnumerable<SettingDescriptor> Members

        IEnumerator<SettingDescriptor> IEnumerable<SettingDescriptor>.GetEnumerator()
        {
            foreach (var pair in _settings)
            {
                foreach (SettingItem item in pair.Value)
                {
                    yield return new SettingDescriptor(pair.Key, item);
                }
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Specifies how to initialize a <see cref="SettingsManager"/>-instance.
        /// </summary>
        public enum SettingsInitialization
        {
            /// <summary>
            /// Initializes only the settings (default value).
            /// </summary>
            OnlySettings = 0,
            /// <summary>
            /// Includes the settings display configuration when initializing.
            /// Should be done only if needed to keep memory and initialization time low.
            /// </summary>
            IncludeDisplayConfiguration = 1,
        }

        #endregion
    }
}
