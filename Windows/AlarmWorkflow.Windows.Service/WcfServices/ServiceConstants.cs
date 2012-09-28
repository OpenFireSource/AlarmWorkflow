using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AlarmWorkflow.Windows.Service.WcfServices
{
    static class ServiceConstants
    {
        private static readonly string ServicesUrlFormat = "http://localhost:{0}/AlarmWorkflow/{1}";
        /// <summary>
        /// Defines the default port for all services.
        /// </summary>
        internal static readonly int ServicesPort = 60002;
        /// <summary>
        /// Specifies the Binding that is used for all services.
        /// </summary>
        internal static readonly Binding ServicesBinding;

        /// <summary>
        /// Initializes the <see cref="ServiceConstants"/> class.
        /// </summary>
        static ServiceConstants()
        {
            ServicesBinding = new BasicHttpBinding()
            {
                //ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                MaxReceivedMessageSize = int.MaxValue
            };
        }

        /// <summary>
        /// Returns the Service-URL for the given service.
        /// </summary>
        /// <param name="serviceName">The name of the servie to get the Service-URL for.</param>
        /// <returns></returns>
        internal static string GetServiceUrl(string serviceName)
        {
            return string.Format(ServiceConstants.ServicesUrlFormat, ServiceConstants.ServicesPort, serviceName);
        }
    }
}
