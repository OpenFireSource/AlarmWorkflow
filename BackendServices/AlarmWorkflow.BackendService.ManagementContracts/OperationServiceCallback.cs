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

using System;
using System.ServiceModel;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.BackendService.ManagementContracts
{
    /// <summary>
    /// Provides a reference implementation of the <see cref="IOperationServiceCallback"/> interface.
    /// </summary>
    [CallbackBehavior()]
    public class OperationServiceCallback : IOperationServiceCallback
    {
        #region Events

        /// <summary>
        /// Occurs when an operation was just acknowledged.
        /// </summary>
        public event Action<int> OperationAcknowledged;

        #endregion

        #region IOperationServiceCallback Members

        void IOperationServiceCallback.OnOperationAcknowledged(int id)
        {
            try
            {
                var copy = OperationAcknowledged;
                if (copy != null)
                {
                    copy(id);
                }
            }
            catch (Exception ex)
            {
                // Callback methods shall always have a no-throw guarantee!
                Logger.Instance.LogException(this, ex);
            }
        }

        #endregion
    }
}
