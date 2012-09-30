using System;

namespace AlarmWorkflow.Job.SQLCEDatabaseJob
{
    partial class OperationData
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationData"/> class.
        /// </summary>
        public OperationData()
        {
            this.ID = Guid.NewGuid();
        }

        #endregion
    }
}
