using System;

namespace AlarmWorkflow.Job.MySqlDatabaseJob.Data
{
    partial class OperationData
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationData"/> class.
        /// </summary>
        public OperationData()
        {
            this.OperationId = Guid.NewGuid();
        }

        #endregion
    }
}
