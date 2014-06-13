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
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Backend.ServiceContracts.Core
{
    /// <summary>
    /// Represents the default fault detail for AlarmWorkflow-services.
    /// </summary>
    [Serializable()]
    public sealed class AlarmWorkflowFaultDetails
    {
        #region Properties

        /// <summary>
        /// Gets/sets the message text.
        /// </summary>
        public string Message { get; set; }

        #endregion

        #region Constructors

        private AlarmWorkflowFaultDetails()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmWorkflowFaultDetails"/> class.
        /// </summary>
        /// <param name="exception">The exception to wrap.</param>
        public AlarmWorkflowFaultDetails(Exception exception)
            : this()
        {
            Assertions.AssertNotNull(exception, "exception");

            this.Message = exception.Message;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new FaultException with our fault details based on an existing exception.
        /// </summary>
        /// <param name="exception">The exception to wrap.</param>
        /// <returns></returns>
        public static FaultException<AlarmWorkflowFaultDetails> CreateFault(Exception exception)
        {
            Assertions.AssertNotNull(exception, "exception");

            if (exception is FaultException<AlarmWorkflowFaultDetails>)
            {
                return (FaultException<AlarmWorkflowFaultDetails>)exception;
            }

            FaultReason reason = new FaultReason(exception.Message);
            FaultCode code = new FaultCode("(No code)");
            AlarmWorkflowFaultDetails detail = new AlarmWorkflowFaultDetails(exception);

            return new FaultException<AlarmWorkflowFaultDetails>(detail, reason, code);
        }

        #endregion
    }
}
