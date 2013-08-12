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

        OperationItem IAlarmWorkflowService.GetOperationById(string operationId)
        {
            int rOperationId = -1;
            if (!int.TryParse(operationId, out rOperationId))
            {
                // Invalid case
                return null;
            }

            Operation operation = _operationStore.GetOperationById(rOperationId);
            if (operation == null)
            {
                return null;
            }

            return new OperationItem(operation);
        }

        #endregion
    }
}