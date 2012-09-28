using System;
using System.Collections.Generic;
using System.ServiceModel;

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
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmWorkflowService"/> class.
        /// </summary>
        public AlarmWorkflowService()
        {

        }

        #endregion

        #region IAlarmWorkflowService Members

        IList<OperationItem> IAlarmWorkflowService.GetOperations(int maxAge, bool onlyNonAcknowledged, int limitAmount)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
