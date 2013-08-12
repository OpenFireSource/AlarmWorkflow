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
using System.ServiceModel.Web;

namespace AlarmWorkflow.Windows.ServiceContracts
{
    /// <summary>
    /// Defines the web service for the Windows-implementation of the AlarmWorkflow application.
    /// </summary>
    [ServiceContract()]
    public interface IAlarmWorkflowService
    {
        /// <summary>
        /// Returns a list containing the Identifiers of all operations using a predefined set of filter criteria.
        /// The real <see cref="OperationItem"/>s can then be retrieved by using <see cref="M:GetOperationById(int)"/>.
        /// </summary>
        /// <param name="maxAge">The maximum age of the operations to retrieve, in minutes. Use 0 (zero) for no maximum age.</param>
        /// <param name="onlyNonAcknowledged">Whether or not only to fetch non-acknowledged "new" operations.</param>
        /// <param name="limitAmount">The amount of operations to retrieve. Higher limits may take longer to fetch. Use 0 (zero) for no limit.</param>
        /// <returns>A list containing the Identifiers of all operations using a predefined set of filter criteria.</returns>
        [OperationContract()]
        [WebGet(UriTemplate = "GetOperationIds/maxAge={maxAge}&ona={onlyNonAcknowledged}&limit={limitAmount}", ResponseFormat = WebMessageFormat.Json)]
        IList<int> GetOperationIds(string maxAge, string onlyNonAcknowledged, string limitAmount);
        /// <summary>
        /// Returns an Operation by its Id. If there is no operation with the given id, null is returned.
        /// </summary>
        /// <param name="operationId">The Id of the operation to get.</param>
        /// <returns>An Operation by its Id. If there is no operation with the given id, null is returned.</returns>
        [OperationContract()]
        [WebGet(UriTemplate = "GetOperationById/id={operationId}", ResponseFormat = WebMessageFormat.Json)]
        OperationItem GetOperationById(string operationId);
    }
}