using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace AlarmWorkflow.Windows.Service.WcfServices
{
    /// <summary>
    /// Defines the contract for the internal, local-machine service used between Windows Service and Clients (not exposed to external).
    /// </summary>
    [ServiceContract()]
    public interface IAlarmWorkflowServiceInternal
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
        IList<int> GetOperationIds(int maxAge, bool onlyNonAcknowledged, int limitAmount);
        /// <summary>
        /// Returns an Operation by its Id. If there is no operation with the given id, null is returned.
        /// </summary>
        /// <param name="operationId">The Id of the operation to get.</param>
        /// <param name="detailLevel">The detail level. Currently supported are 0 (minimum detail) and 1 (full detail). For mobile phones, 0 is recommended; otherwise, 1.</param>
        /// <returns>An Operation by its Id. If there is no operation with the given id, null is returned.</returns>
        [OperationContract()]
        OperationItem GetOperationById(int operationId, OperationItemDetailLevel detailLevel);
        /// <summary>
        /// Acknowledges the given operation. If the operation is already acknowledged, it will do nothing.
        /// Setting an operation to be acknowledged will not cause it to be displayed in the UIs (an acknowledged operation is "done").
        /// </summary>
        /// <param name="operationId">The Id of the <see cref="Operation"/> to set to "acknowledged".</param>
        [OperationContract()]
        void AcknowledgeOperation(int operationId);
    }
}