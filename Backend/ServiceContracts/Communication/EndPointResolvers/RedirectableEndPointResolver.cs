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

using AlarmWorkflow.Backend.ServiceContracts.ServiceDefinition;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Backend.ServiceContracts.Communication.EndPointResolvers
{
    /// <summary>
    /// Represents an <see cref="IEndPointResolver"/> which can be configured to connect to a specific end point.
    /// </summary>
    public sealed class RedirectableEndPointResolver : IEndPointResolver
    {
        #region Fields

        private readonly string _endPoint;

        #endregion

        #region Constructors

        private RedirectableEndPointResolver()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectableEndPointResolver"/> class.
        /// </summary>
        /// <param name="backendConfigurator">The backend configurator to use to retrieve the information from.</param>
        public RedirectableEndPointResolver(IBackendConfigurator backendConfigurator)
            : this()
        {
            Assertions.AssertNotNull(backendConfigurator, "backendConfigurator");

            _endPoint = backendConfigurator.Get("ServerHostAddress");
        }

        #endregion

        #region IEndPointResolver Members

        string IEndPointResolver.GetServerAddress()
        {
            return _endPoint;
        }

        #endregion
    }
}
