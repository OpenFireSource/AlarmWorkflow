using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Extensibility
{
    /// <summary>
    /// Defines a means for a type that stores <see cref="Operation"/>
    /// </summary>
    public interface IOperationStore : IExtensionObject
    {
        /// <summary>
        /// Returns a list containing all operations using a predefined set of filter criteria.
        /// </summary>
        /// <param name="maxAge">The maximum age of the operations to retrieve, in minutes. Use 0 (zero) for no maximum age.</param>
        /// <param name="onlyNonAcknowledged">Whether or not only to fetch non-acknowledged "new" operations.</param>
        /// <param name="limitAmount">The amount of operations to retrieve. Higher limits may take longer to fetch. Use 0 (zero) for no limit.</param>
        /// <returns>A list containing all operations using a predefined set of filter criteria.</returns>
        IList<Operation> GetOperations(int maxAge, bool onlyNonAcknowledged, int limitAmount);
        /// <summary>
        /// Acknowledges the given operation. If the operation is already acknowledged, it will do nothing.
        /// Setting an operation to be acknowledged will not cause it to be displayed in the UIs (an acknowledged operation is "done").
        /// </summary>
        /// <param name="operationId">The Id of the <see cref="Operation"/> to set to "acknowledged".</param>
        void AcknowledgeOperation(int operationId);
    }
}
