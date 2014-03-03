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

using System.ServiceModel;

namespace AlarmWorkflow.BackendService.ManagementContracts
{
    /// <summary>
    /// Defines methods for the callback interface used by the <see cref="IOperationService"/>.
    /// </summary>
    public interface IOperationServiceCallback
    {
        /// <summary>
        /// Called when an operation was acknowledged.
        /// </summary>
        /// <param name="id">The id of the operation that was acknowledged.</param>
        [OperationContract(IsOneWay = true)]
        void OnOperationAcknowledged(int id);
    }
}
