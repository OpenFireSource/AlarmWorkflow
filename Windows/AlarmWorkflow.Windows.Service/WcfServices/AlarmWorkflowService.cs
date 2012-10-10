using System;
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
        public AlarmWorkflowService(AlarmWorkflowEngine parent)
        {
            _operationStore = parent.GetOperationStore();
        }

        #endregion

        #region IAlarmWorkflowService Members

        IList<int> IAlarmWorkflowService.GetOperationIds(string maxAge, string onlyNonAcknowledged, string limitAmount)
        {
            // Cast to correct values
            int rMaxAge = 8;
            bool rOnlyNonAcknowledged = true;
            int rLimitAmount = 10;

            // Try to parse the values (if one fails to pass just go on and take the default values from above)
            int.TryParse(maxAge, out rMaxAge);
            bool.TryParse(onlyNonAcknowledged, out rOnlyNonAcknowledged);
            int.TryParse(limitAmount, out rLimitAmount);

            return _operationStore.GetOperationIds(rMaxAge, rOnlyNonAcknowledged, rLimitAmount);
        }

        OperationItem IAlarmWorkflowService.GetOperationById(string operationId, string detailLevel)
        {
            int rDetailLevel = 0;
            int rOperationId = -1;
            if (!int.TryParse(operationId, out rOperationId))
            {
                // Invalid case
                return null;
            }
            int.TryParse(detailLevel, out rDetailLevel);

            Operation operation = _operationStore.GetOperationById(rOperationId);
            if (operation == null)
            {
                return null;
            }

            OperationItemDetailLevel detail = OperationItemDetailLevel.Minimum;
            if (rDetailLevel >= 0 && rDetailLevel <= 1)
            {
                detail = (OperationItemDetailLevel)rDetailLevel;
            }

            return new OperationItem(operation, detail);
        }

        void IAlarmWorkflowService.AcknowledgeOperation(string operationId)
        {
            int rOperationId = -1;
            if (!int.TryParse(operationId, out rOperationId))
            {
                // Invalid case
                return;
            }

            _operationStore.AcknowledgeOperation(rOperationId);
        }

        #endregion
    }
}
