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

namespace AlarmWorkflow.Shared.ObjectExpressions
{
    /// <summary>
    /// Represents a specialized exception that is thrown in conjunction with processing expressions.
    /// </summary>
    public abstract class ExpressionException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionException"/> class.
        /// </summary>
        protected ExpressionException()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        protected ExpressionException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        protected ExpressionException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        #endregion
    }
}