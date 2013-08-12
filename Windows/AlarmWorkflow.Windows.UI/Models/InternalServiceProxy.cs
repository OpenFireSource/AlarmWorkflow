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

using System.ServiceModel;
using System.Xml;
using AlarmWorkflow.Windows.ServiceContracts;

namespace AlarmWorkflow.Windows.UI.Models
{
    /// <summary>
    /// Proxy to the IAlarmWorkflowServiceInternal, used between Windows Service and clients.
    /// </summary>
    sealed class InternalServiceProxy
    {
        #region Methods

        /// <summary>
        /// Creates a new service wrapper for this type.
        /// </summary>
        /// <returns></returns>
        public static WrappedService<IAlarmWorkflowServiceInternal> GetServiceInstance()
        {
            // Service names ought to start with an 'I'
            EndpointAddress endpointAddress = new EndpointAddress("net.pipe://localhost/alarmworkflow/service");
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport)
            {
                MaxReceivedMessageSize = int.MaxValue,
                ReaderQuotas = XmlDictionaryReaderQuotas.Max,
            };

            ChannelFactory<IAlarmWorkflowServiceInternal> d = new ChannelFactory<IAlarmWorkflowServiceInternal>(binding, endpointAddress);
            return new WrappedService<IAlarmWorkflowServiceInternal>(d.CreateChannel());
        }

        #endregion
    }
}