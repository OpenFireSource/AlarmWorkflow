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

        IList<int> IAlarmWorkflowService.GetOperationIds(int maxAge, bool onlyNonAcknowledged, int limitAmount)
        {
            return _operationStore.GetOperationIds(maxAge, onlyNonAcknowledged, limitAmount);
        }

        OperationItem IAlarmWorkflowService.GetOperationById(int operationId)
        {
            Operation operation = _operationStore.GetOperationById(operationId);
            if (operation == null)
            {
                return null;
            }

            return new OperationItem(operation);
        }

        void IAlarmWorkflowService.AcknowledgeOperation(int operationId)
        {
            _operationStore.AcknowledgeOperation(operationId);
        }

        #endregion
    }
}
