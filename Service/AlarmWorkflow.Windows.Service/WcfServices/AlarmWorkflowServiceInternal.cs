// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.ServiceModel;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Engine;
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

        OperationItem IAlarmWorkflowServiceInternal.GetOperationById(int operationId)
        {
            Operation operation = _operationStore.GetOperationById(operationId);
            if (operation == null)
            {
                return null;
            }

            return new OperationItem(operation);
        }

        void IAlarmWorkflowServiceInternal.AcknowledgeOperation(int operationId)
        {
            _operationStore.AcknowledgeOperation(operationId);
        }

        #endregion
    }
}