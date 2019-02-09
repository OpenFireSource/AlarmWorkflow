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
using System.Linq;
using AlarmWorkflow.Backend.Data.Types;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.Management
{
    static class DataExtensions
    {
        #region Methods

        internal static OperationData ToData(this Operation operation)
        {
            var data = new OperationData
            {
                Guid = operation.OperationGuid,
                IsAcknowledged = operation.IsAcknowledged,
                OperationNumber = operation.OperationNumber,
                IncomeAt = operation.TimestampIncome,
                AlarmAt = operation.Timestamp,
                Comment = operation.Comment,
                Messenger = operation.Messenger,
                Plan = operation.OperationPlan,
                Picture = operation.Picture,
                Priority = operation.Priority,
                Loops = operation.Loops.ToString(),
                Einsatzort = operation.Einsatzort,
                Zielort = operation.Zielort,
                Keywords = operation.Keywords,
                CustomData = JsonHelper.ToJson(operation.CustomData)
            };

            foreach (OperationResource item in operation.Resources)
            {
                data.Resources.Add(item.ToData());
            }

            return data;
        }

        internal static Operation ToOperation(this OperationData data)
        {
            var operation = new Operation()
            {
                Id = data.Id,
                IsAcknowledged = data.IsAcknowledged,
                OperationGuid = data.Guid,
                OperationNumber = data.OperationNumber,
                Timestamp = data.AlarmAt,
                TimestampIncome = data.IncomeAt,
                OperationPlan = data.Plan,
                Comment = data.Comment,
                Messenger = data.Messenger,
                Picture = data.Picture,
                Priority = data.Priority,
                Einsatzort = data.Einsatzort,
                Zielort = data.Zielort,
                Keywords = data.Keywords,
                Loops = new OperationLoopCollection(data.Loops),
                CustomData = JsonHelper.FromJson<IDictionary<string, object>>(data.CustomData, new Dictionary<string, object>()),
            };

            foreach (OperationResourceData item in data.Resources)
            {
                operation.Resources.Add(item.ToOperationResource());
            }

            return operation;
        }

        internal static OperationResourceData ToData(this OperationResource operationResource)
        {
            OperationResourceData data = new OperationResourceData();

            data.FullName = operationResource.FullName;
            data.Timestamp = operationResource.Timestamp;
            data.Equipment = CsvHelper.ToCsvLine(operationResource.RequestedEquipment);

            return data;
        }

        internal static OperationResource ToOperationResource(this OperationResourceData data)
        {
            return new OperationResource()
            {
                FullName = data.FullName,
                Timestamp = data.Timestamp,
                RequestedEquipment = CsvHelper.FromCsvLine(data.Equipment).ToList(),
            };
        }

        #endregion
    }
}