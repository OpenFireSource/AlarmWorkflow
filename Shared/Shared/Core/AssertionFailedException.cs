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

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Represents the exception that occurs if an assertion has failed.
    /// </summary>
    [Serializable()]
    public sealed class AssertionFailedException : Exception
    {
        #region Properties

        /// <summary>
        /// Gets the type of the assertion.
        /// </summary>
        public AssertionType Assertion { get; private set; }
        /// <summary>
        /// Gets the affected parameter names.
        /// </summary>
        public string[] AffectedParameterNames { get; private set; }

        /// <summary>
        /// Gets the message describing this exception.
        /// </summary>
        public override string Message
        {
            get { return GetExceptionMessage(Assertion, AffectedParameterNames); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertionFailedException"/> class.
        /// </summary>
        /// <param name="assertion">Type of the assertion.</param>
        /// <param name="affectedParameterNames">The affected parameter names.</param>
        public AssertionFailedException(AssertionType assertion, string[] affectedParameterNames)
            : this(Properties.Resources.AssertionFailedGenericMessage, assertion, affectedParameterNames)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertionFailedException"/> class.
        /// </summary>
        /// <param name="message">The message to display if the assertion has failed.</param>
        /// <param name="assertion">Type of the assertion.</param>
        /// <param name="affectedParameterNames">The affected parameter names.</param>
        public AssertionFailedException(string message, AssertionType assertion, string[] affectedParameterNames)
            : base(message)
        {
            this.Assertion = assertion;
            this.AffectedParameterNames = affectedParameterNames;
        }

        #endregion

        #region Methods

        private static string GetExceptionMessage(AssertionType assertion, string[] affectedParameterNames)
        {
            string format=null;
            switch (assertion)
            {
                case AssertionType.AssertNotNull:
                    format= Properties.Resources.AssertionFailedExceptionNotNull;
                    break;
                case AssertionType.AssertNotEmpty:
                    format= Properties.Resources.AssertionFailedExceptionNotEmpty;
                    break;
                case AssertionType.Invalid:
                default:
                    format= Properties.Resources.AssertionFailedGenericMessage;
                    break;
            }

            return string.Format(format, affectedParameterNames);
        }

        #endregion
    }
}
