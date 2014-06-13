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
using System.ServiceModel;

namespace AlarmWorkflow.Backend.ServiceContracts.Core
{
    /// <summary>
    /// Defines mechanisms for a service that is exposed over WCF.
    /// See documentation for further information.
    /// </summary>
    /// <remarks>Use this interface for your service if you want to allow thirds to communicate with your service.
    /// Exposed services are by default on a per-session basis.</remarks>
    [ServiceContract()]
    public interface IExposedService : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> that can be used to access the service-internal services.
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Performs a ping to the service.
        /// </summary>
        [OperationContract()]
        void Ping();
    }
}
