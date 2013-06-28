using System.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Database.Data
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
