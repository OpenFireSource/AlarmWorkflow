using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace AlarmWorkflow.Windows.Service.WcfServices
{
    /// <summary>
    /// Defines the web service for the Windows-implementation of the AlarmWorkflow application.
    /// </summary>
    [ServiceContract()]
    public interface IAlarmWorkflowService
    {
        /// <summary>
        /// Returns a list containing the Identifiers of all operations using a predefined set of filter criteria.
        /// The real <see cref="OperationItem"/>s can then be retrieved by using <see cref="M:GetOperationById(int)"/>.
        /// </summary>
        /// <param name="maxAge">The maximum age of the operations to retrieve, in minutes. Use 0 (zero) for no maximum age.</param>
        /// <param name="onlyNonAcknowledged">Whether or not only to fetch non-acknowledged "new" operations.</param>
        /// <param name="limitAmount">The amount of operations to retrieve. Higher limits may take longer to fetch. Use 0 (zero) for no limit.</param>
        /// <returns>A list containing the Identifiers of all operations using a predefined set of filter criteria.</returns>
        [OperationContract()]
        [WebGet(UriTemplate = "GetOperationIds/{maxAge}&{onlyNonAcknowledged}&{limitAmount}", ResponseFormat = WebMessageFormat.Json)]
        IList<int> GetOperationIds(string maxAge, string onlyNonAcknowledged, string limitAmount);
        /// <summary>
        /// Returns an Operation by its Id. If there is no operation with the given id, null is returned.
        /// </summary>
        /// <param name="operationId">The Id of the operation to get.</param>
        /// <returns>An Operation by its Id. If there is no operation with the given id, null is returned.</returns>
        [OperationContract()]
        [WebGet(UriTemplate = "GetOperationById/{operationId}", ResponseFormat = WebMessageFormat.Json)]
        OperationItem GetOperationById(string operationId);
        /// <summary>
        /// Acknowledges the given operation. If the operation is already acknowledged, it will do nothing.
        /// Setting an operation to be acknowledged will not cause it to be displayed in the UIs (an acknowledged operation is "done").
        /// </summary>
        /// <param name="operationId">The Id of the <see cref="Operation"/> to set to "acknowledged".</param>
        [OperationContract()]
        [WebGet(UriTemplate = "AcknowledgeOperation/{operationId}", ResponseFormat = WebMessageFormat.Json)]
        void AcknowledgeOperation(string operationId);
    }
}