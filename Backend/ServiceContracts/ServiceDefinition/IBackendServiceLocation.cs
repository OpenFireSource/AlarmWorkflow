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

namespace AlarmWorkflow.Backend.ServiceContracts.ServiceDefinition
{
    /// <summary>
    /// Defines properties that specify the information used to connect to a backend service.
    /// </summary>
    public interface IBackendServiceLocation
    {
        /// <summary>
        /// Gets the type of the contract that this service implements.
        /// </summary>
        Type ContractType { get; }
        /// <summary>
        /// Gets the type that represents the service implementation.
        /// </summary>
        Type ServiceType { get; }
        /// <summary>
        /// Gets/sets the binding that this service uses.
        /// </summary>
        SupportedBinding Binding { get; }
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the name of the assembly in which the <see cref="ServiceType"/> is located in.
        /// </summary>
        string AssemblyName { get; }
    }
}
