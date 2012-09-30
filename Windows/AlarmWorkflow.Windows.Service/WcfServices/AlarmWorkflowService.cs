using System.Collections.Generic;
using System.ServiceModel;
using AlarmWorkflow.Shared;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Windows.Service.WcfServices
{
    /// <summary>
    /// Realizes the AlarmWorkflowService implementation.
    /// </summary>
#if DEBUG
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
#else
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
#endif
    sealed class AlarmWorkflowService : IAlarmWorkflowService
    {
        #region Fields

        private IOperationStore _operationStore;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmWorkflowService"/> class.
        /// </summary>
        /// <param name="parent"></param>
        public AlarmWorkflowService(AlarmworkflowClass parent)
        {
            _operationStore = parent.GetOperationStore();
        }

        #endregion

        #region IAlarmWorkflowService Members

        IList<OperationItem> IAlarmWorkflowService.GetOperations(int maxAge, bool onlyNonAcknowledged, int limitAmount)
        {
            var data = _operationStore.GetOperations(maxAge, onlyNonAcknowledged, limitAmount);
            List<OperationItem> operations = new List<OperationItem>(data.Count);
            foreach (Operation item in data)
            {
                operations.Add(new OperationItem(item));
            }
            return operations;
        }

        void IAlarmWorkflowService.AcknowledgeOperation(int operationId)
        {
            _operationStore.AcknowledgeOperation(operationId);
        }

        #endregion
    }
}
