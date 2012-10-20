using System.ServiceModel;
using System.Xml;
using AlarmWorkflow.Windows.Service.WcfServices;

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
