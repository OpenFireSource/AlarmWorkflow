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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.Backend.ServiceContracts.ServiceDefinition;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Backend.ServiceContracts.Communication
{
    /// <summary>
    /// Represents a type of <see cref="IServiceLocator"/> that discovers services based on a configuration file.
    /// </summary>
    public sealed class ConfigurableServiceLocator : IServiceLocator
    {
        #region Constants

        /// <summary>
        /// Defines the name of the file that describes all known services.
        /// This public static field is read-only.
        /// </summary>
        public static readonly string ConfigKeyServiceLocation = "Wcf.ServiceLocations";

        #endregion

        #region Fields

        private readonly ReadOnlyCollection<IBackendServiceLocation> _serviceLocationCache;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableServiceLocator"/> class,
        /// and retrieves the configuration file from the backend configuration.
        /// </summary>
        /// <exception cref="AlarmWorkflow.Shared.Core.AssertionFailedException">The parameters were invalid.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The configuration file was not found.</exception>
        /// <exception cref="System.Xml.Schema.XmlSchemaValidationException">The configuration file was malformed.</exception>
        public ConfigurableServiceLocator()
            : this(BackendConfiguration.Values[ConfigKeyServiceLocation])
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableServiceLocator"/> class with a custom configuration file path.
        /// </summary>
        /// <param name="configurationFile">The name of the configuration file. If this file path is relative, it is assumed relative to the working directory.</param>
        /// <exception cref="AlarmWorkflow.Shared.Core.AssertionFailedException">The parameters were invalid.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The configuration file was not found.</exception>
        /// <exception cref="System.Xml.Schema.XmlSchemaValidationException">The configuration file was malformed.</exception>
        public ConfigurableServiceLocator(string configurationFile)
        {
            Assertions.AssertNotEmpty(configurationFile, "configurationFile");

            if (!Path.IsPathRooted(configurationFile))
            {
                configurationFile = Path.Combine(Utilities.GetWorkingDirectory(), configurationFile);
            }

            if (!File.Exists(configurationFile))
            {
                throw new FileNotFoundException(Properties.Resources.ServiceLocationFileNotPresent, configurationFile);
            }

            _serviceLocationCache = new ReadOnlyCollection<IBackendServiceLocation>(BuildServiceLocationCache(configurationFile).ToList());
        }

        #endregion

        #region Methods

        private IEnumerable<IBackendServiceLocation> BuildServiceLocationCache(string fileName)
        {            
            XDocument doc = XDocument.Load(fileName);
            doc.ValidateXml(Properties.Resources.BackendServiceLocationSchema);
            foreach (XElement service in doc.Root.Elements("service"))
            {
                BackendServiceLocation item = new BackendServiceLocation();
                item.ContractType = Type.GetType(service.Attribute("contract").Value);
                item.ServiceType = Type.GetType(service.Attribute("service").Value);
                item.Binding = (SupportedBinding)Enum.Parse(typeof(SupportedBinding), service.TryGetAttributeValue("binding", SupportedBinding.NetTcp.ToString()), false);
                yield return item;
            }
        }

        #endregion

        #region IServiceLocator Members

        IEnumerable<IBackendServiceLocation> IServiceLocator.GetKnownServices()
        {
            return _serviceLocationCache;
        }

        #endregion
    }
}
