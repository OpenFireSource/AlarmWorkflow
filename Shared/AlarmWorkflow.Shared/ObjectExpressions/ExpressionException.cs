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
