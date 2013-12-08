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

using System.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.Management.Data
{
    partial class OperationResourceData
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResourceData"/> class.
        /// </summary>
        public OperationResourceData()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResourceData"/> class,
        /// and copies the contents of the given <see cref="OperationResource"/> to this entity.
        /// </summary>
        /// <param name="operationResource">The resource to copy its contents.</param>
        public OperationResourceData(OperationResource operationResource)
            : this()
        {
            this.FullName = operationResource.FullName;
            this.Timestamp = operationResource.Timestamp;
            this.EquipmentCsv = CsvHelper.ToCsvLine(operationResource.RequestedEquipment);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Copies the contents of this entity to a new instance of the <see cref="OperationResource"/> class.
        /// </summary>
        /// <returns></returns>
        public OperationResource ToOperationResource()
        {
            return new OperationResource()
            {
                FullName = this.FullName,
                Timestamp = this.Timestamp,
                RequestedEquipment = CsvHelper.FromCsvLine(this.EquipmentCsv).ToList(),
            };
        }

        #endregion
    }
}