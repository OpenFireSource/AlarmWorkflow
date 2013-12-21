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

using System.Collections.Generic;
using AlarmWorkflow.Backend.ServiceContracts.ServiceDefinition;

namespace AlarmWorkflow.Backend.ServiceContracts.Communication
{
    /// <summary>
    /// Defines a method that can be used to fetch location information of services to connect to.
    /// </summary>
    public interface IServiceLocator
    {
        /// <summary>
        /// Returns all services known to this implementation that can be connected to.
        /// </summary>
        /// <returns>All services known to this implementation that can be connected to.</returns>
        IEnumerable<IBackendServiceLocation> GetKnownServices();
    }
}
