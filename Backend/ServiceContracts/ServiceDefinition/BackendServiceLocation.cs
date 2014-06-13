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
using System.Diagnostics;

namespace AlarmWorkflow.Backend.ServiceContracts.ServiceDefinition
{
    [DebuggerDisplay("Contract = {ContractType.FullName}, Service = {ServiceType.FullName}")]
    class BackendServiceLocation : IBackendServiceLocation
    {
        #region IBackendServiceLocation Members

        /// <summary>
        /// Gets the type of the contract that this service implements.
        /// </summary>
        public Type ContractType { get; set; }

        /// <summary>
        /// Gets the type that represents the service implementation.
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// Gets/sets the binding that this service uses.
        /// </summary>
        public SupportedBinding Binding { get; set; }

        /// <summary>
        /// Gets the name of the service.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>The service name is inferred from the <see cref="ServiceType"/> by removing the first character (the 'I' of 'IExampleService' for example).</remarks>
        public string Name
        {
            get { return ContractType.Name.Remove(0, 1); }
        }
        /// <summary>
        /// Gets the name of the assembly in which the <see cref="ServiceType"/> is located in.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>This is automatically inferred from the <see cref="ServiceType"/>.</remarks>
        public string AssemblyName
        {
            get { return ServiceType.Assembly.FullName; }
        }

        #endregion
    }
}
