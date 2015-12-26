﻿// This file is part of AlarmWorkflow.
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
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.Shared.Core;
using System;

namespace AlarmWorkflow.BackendService.ManagementContracts
{
    /// <summary>
    /// Defines the methods of the operation service, which manages all operations.
    /// </summary>
    public interface IOperationServiceInternal : IInternalService
    {
        /// <summary>
        /// Raised when an operation was acknowledged.
        ///  </summary>
        event Action<int> OperationAcknowledged;
        /// <summary>
        /// Raised when an operation was added.
        /// </summary>
        event Action<Operation> NewOperation;
        /// <summary>
        /// Determines whether or not an <see cref="Operation"/> with the specified operation number does already exist.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>This is the case if the operation was already processed some time ago. For example, if you are running tests
        /// and place the same native fax in the incoming folder multiple times.</remarks>
        /// <param name="operationNumber">The operation number to use for checking existence.</param>
        /// <returns>A boolean value indicating whether or not an <see cref="Operation"/> with the specified operation number does already exist.</returns>
        bool ExistsOperation(string operationNumber);
        /// <summary>
        /// Stores the given operation in the database.
        /// </summary>
        /// <param name="operation">The operation to store.</param>
        /// <returns></returns>
        Operation StoreOperation(Operation operation);
        /// <summary>
        /// Returns a list containing the Identifiers of all operations using a predefined set of filter criteria.
        /// The real <see cref="Operation"/>s can then be retrieved by using <see cref="M:GetOperationById(int)"/>.
        /// </summary>
        /// <param name="maxAge">The maximum age of the operations to retrieve, in minutes. Use 0 (zero) for no maximum age.</param>
        /// <param name="onlyNonAcknowledged">Whether or not only to fetch non-acknowledged "new" operations.</param>
        /// <param name="limitAmount">The amount of operations to retrieve. Higher limits may take longer to fetch. Use 0 (zero) for no limit.</param>
        /// <returns>A list containing the Identifiers of all operations using a predefined set of filter criteria.</returns>
        IList<int> GetOperationIds(int maxAge, bool onlyNonAcknowledged, int limitAmount);
        /// <summary>
        /// Returns an Operation by its Id. If there is no operation with the given id, null is returned.
        /// </summary>
        /// <param name="operationId">The Id of the operation to get.</param>
        /// <returns>An Operation by its Id. If there is no operation with the given id, null is returned.</returns>
        Operation GetOperationById(int operationId);
        /// <summary>
        /// Acknowledges the given operation. If the operation is already acknowledged, it will do nothing.
        /// Setting an operation to be acknowledged will not cause it to be displayed in the UIs (an acknowledged operation is "done").
        /// </summary>
        /// <param name="operationId">The Id of the Operation to set to "acknowledged".</param>
        void AcknowledgeOperation(int operationId);
    }
}