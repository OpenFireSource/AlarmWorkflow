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

namespace AlarmWorkflow.Backend.ServiceContracts.Core
{
    /// <summary>
    /// Defines mechanisms for a service that is hosted by the Service, but not exposed over WCF.
    /// See documentation for further information.
    /// </summary>
    /// <remarks>Use this interface for your service if you want to benefit from easy-to-use inter-service communication with other service,
    /// but don't want to expose the details to thirds over the wire.</remarks>
    public interface IInternalService : IDisposable
    {
        /// <summary>
        /// Initializes this service after being added to the service manager.
        /// </summary>
        /// <param name="serviceProvider">The service provider to set.</param>
        void Initialize(IServiceProvider serviceProvider);
        /// <summary>
        /// Called when the parent service is iterating through all services to signal them they can start.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>Any exception thrown here will cause the service to fail. It is encouraged to throw exceptions as soon as possible,
        /// so that the service just fails outright instead of "working" in a broken state!</remarks>
        void OnStart();
        /// <summary>
        /// Called when the parent service is iterating through all services - in inverse order - to signal them they can stop.
        /// </summary>
        void OnStop();
    }
}
