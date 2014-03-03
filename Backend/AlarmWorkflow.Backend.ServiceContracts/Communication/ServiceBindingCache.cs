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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Linq;
using AlarmWorkflow.Backend.ServiceContracts.ServiceDefinition;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Backend.ServiceContracts.Communication
{
    /// <summary>
    /// Provides helper methods to retrieve the correct binding for accessing a service.
    /// </summary>
    internal static class ServiceBindingCache
    {
        #region Constants

        private const string ConfigKeyServiceLocation = "Wcf.ServiceLocations";

        #endregion

        #region Fields

        private static readonly ReadOnlyCollection<IBackendServiceLocation> _serviceLocationCache;
        private static readonly Dictionary<Type, Binding> _bindingCache;

        #endregion

        #region Constructors

        static ServiceBindingCache()
        {
            _serviceLocationCache = new ReadOnlyCollection<IBackendServiceLocation>(BuildServiceLocationCache().ToList());
            _bindingCache = new Dictionary<Type, Binding>();
        }

        #endregion

        #region Methods

        private static IEnumerable<IBackendServiceLocation> BuildServiceLocationCache()
        {
            string fileName = ServiceFactory.BackendConfigurator.Get(ConfigKeyServiceLocation);
            fileName = Path.Combine(Utilities.GetWorkingDirectory(), fileName);

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(Properties.Resources.ServiceLocationFileNotPresent, fileName);
            }

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

        internal static IEnumerable<IBackendServiceLocation> GetServiceLocations()
        {
            return _serviceLocationCache;
        }

        /// <summary>
        /// Returns the <see cref="Binding"/> that will be used for the service of the given type.
        /// </summary>
        /// <param name="contractType">The service contract type (must be an interface) to get the binding for.</param>
        /// <returns></returns>
        public static Binding GetBindingForContractType(Type contractType)
        {
            Assertions.AssertNotNull(contractType, "contractType");

            if (!_bindingCache.ContainsKey(contractType))
            {
                _bindingCache[contractType] = FindBindingForContractType(contractType);
            }
            return _bindingCache[contractType];
        }

        private static Binding FindBindingForContractType(Type contractType)
        {
            IBackendServiceLocation serviceLocation = _serviceLocationCache.FirstOrDefault(item => item.ContractType == contractType);
            if (serviceLocation == null)
            {
                throw new InvalidOperationException(string.Format(Properties.Resources.NoAttributeForServiceContractTypeFound, contractType));
            }

            Binding binding = null;
            switch (serviceLocation.Binding)
            {
                case SupportedBinding.NetTcp:
                    binding = new NetTcpBinding()
                    {
                        MaxReceivedMessageSize = int.MaxValue,
                        ReaderQuotas = XmlDictionaryReaderQuotas.Max,
                    };
                    break;
                default:
                    throw new InvalidOperationException(string.Format(Properties.Resources.InvalidSupportedBindingValue, serviceLocation.Binding));
            }

            return binding;
        }

        #endregion
    }
}
