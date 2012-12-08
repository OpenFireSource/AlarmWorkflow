using System.Collections.Generic;
using System.ServiceModel;
using AlarmWorkflow.Shared;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Windows.ServiceContracts;

namespace AlarmWorkflow.Windows.Service.WcfServices
{
    /// <summary>
    /// Realizes the AlarmWorkflowServiceInternal implementation.
    /// </summary>
#if DEBUG
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
#else
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
#endif
    sealed class AlarmWorkflowServiceInternal : IAlarmWorkflowServiceInternal
    {
        #region Fields

        private IOperationStore _operationStore;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmWorkflowServiceInternal"/> class.
        /// </summary>
        /// <param name="parent"></param>
        public AlarmWorkflowServiceInternal(AlarmWorkflowEngine parent)
        {
            _operationStore = parent.GetOperationStore();
        }

        #endregion

        #region IAlarmWorkflowServiceInternal Members

        IList<int> IAlarmWorkflowServiceInternal.GetOperationIds(int maxAge, bool onlyNonAcknowledged, int limitAmount)
        {
            return _operationStore.GetOperationIds(maxAge, onlyNonAcknowledged, limitAmount);
        }

        OperationItem IAlarmWorkflowServiceInternal.GetOperationById(int operationId, OperationItemDetailLevel detailLevel)
        {
            Operation operation = _operationStore.GetOperationById(operationId);
            if (operation == null)
            {
                return null;
            }

            return new OperationItem(operation, detailLevel);
        }

        void IAlarmWorkflowServiceInternal.AcknowledgeOperation(int operationId)
        {
            _operationStore.AcknowledgeOperation(operationId);
        }

        #endregion
    }
}
