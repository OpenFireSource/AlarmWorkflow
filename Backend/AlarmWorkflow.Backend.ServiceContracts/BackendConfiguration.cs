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

namespace AlarmWorkflow.Backend.ServiceContracts
{
    /// <summary>
    /// Represents the global backend configuration.
    /// </summary>
    internal sealed class BackendConfiguration
    {
        #region Constants

        private const string BackendConfigFileName = "Backend.config";
        /// <summary>
        /// Defines the default port to use for services bound to NetTcp, if no port was configured.
        /// </summary>
        public const int ServerNetTcpPortDefault = 60000;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of setting values from the configuration file.
        /// </summary>
        public static NameValueCollection Values { get; private set; }

        /// <summary>
        /// Gets the configured port to use for services bound to NetTcp.
        /// If no port is configured, the default port (see <see cref="ServerNetTcpPortDefault"/>) will be returned.
        /// </summary>
        public static int ServerNetTcpPort
        {
            get
            {
                string port = Values["Server.NetTcpPort"];
                if (!string.IsNullOrWhiteSpace(port))
                {
                    return int.Parse(port);
                }
                return ServerNetTcpPortDefault;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="BackendConfiguration"/> class.
        /// </summary>
        static BackendConfiguration()
        {
            Values = new NameValueCollection();
            LoadBackendConfig();
        }

        #endregion

        #region Methods

        private static void LoadBackendConfig()
        {
            string filePath = Path.Combine(Utilities.GetWorkingDirectory(), BackendConfigFileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(Properties.Resources.BackendConfigFileNotPresent, filePath);
            }

            try
            {
                XDocument doc = XDocument.Load(filePath);
                foreach (XElement item in doc.Root.Element("appSettings").Elements("add"))
                {
                    string key = item.Attribute("key").Value;
                    string value = item.Attribute("value").Value;
                    Values[key] = value;
                }

                Logger.Instance.LogFormat(LogType.Trace, typeof(BackendConfiguration), Properties.Resources.BackendConfigurationSuccessfullyLoaded);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(typeof(BackendConfiguration), ex);
            }
        }

        private static void ParseBackendConfig()
        {

        }

        #endregion
    }
}
