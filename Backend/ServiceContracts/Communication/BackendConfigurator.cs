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
using System.Collections.Specialized;
using System.IO;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Backend.ServiceContracts.Communication
{
    /// <summary>
    /// Represents the global backend configuration.
    /// </summary>
    public sealed class BackendConfigurator : IBackendConfigurator
    {
        #region Constants

        private const string BackendConfigFileName = "Backend.config";
        private static readonly string BackendConfigFilePath = Path.Combine(Utilities.GetWorkingDirectory(), BackendConfigFileName);

        #endregion

        #region Fields

        private readonly NameValueCollection _values;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BackendConfigurator"/> class,
        /// and loads the backend.config from its default path.
        /// </summary>
        /// <exception cref="System.IO.FileNotFoundException">The file was not found.</exception>
        public BackendConfigurator()
            : this(BackendConfigFilePath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackendConfigurator"/> class.
        /// </summary>
        /// <param name="fileName">The path to the backend configuration file.</param>
        /// <exception cref="System.IO.FileNotFoundException">The file was not found.</exception>
        public BackendConfigurator(string fileName)
        {
            _values = new NameValueCollection();
            LoadBackendConfig(fileName);
        }

        #endregion

        #region Methods

        private void LoadBackendConfig(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(Properties.Resources.BackendConfigFileNotPresent, fileName);
            }

            try
            {
                XDocument doc = XDocument.Load(fileName);
                foreach (XElement item in doc.Root.Element("appSettings").Elements("add"))
                {
                    string key = item.Attribute("key").Value;
                    string value = item.Attribute("value").Value;
                    _values[key] = value;
                }

                Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.BackendConfigurationSuccessfullyLoaded);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
            }
        }

        #endregion

        #region IBackendConfigurator Members

        string IBackendConfigurator.Get(string key)
        {
            return _values[key];
        }

        #endregion
    }
}
