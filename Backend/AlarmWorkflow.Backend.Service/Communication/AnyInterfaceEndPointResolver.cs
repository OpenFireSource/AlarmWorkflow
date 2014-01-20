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

using System.Net;
using AlarmWorkflow.Backend.ServiceContracts.Communication;

namespace AlarmWorkflow.Backend.Service.Communication
{
    /// <summary>
    /// Represents an <see cref="IEndPointResolver"/> that is used for the Service to host all services on any NIC.
    /// </summary>
    class AnyInterfaceEndPointResolver : IEndPointResolver
    {
        #region IEndPointResolver Members

        string IEndPointResolver.GetServerAddress()
        {
            return IPAddress.Any.ToString();
        }

        #endregion
    }
}
