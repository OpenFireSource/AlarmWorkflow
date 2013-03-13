using AlarmWorkflow.Shared.Properties;

namespace AlarmWorkflow.Shared.ObjectExpressions
{
    /// <summary>
    /// Represents an exception that is thrown when an expression was in an invalid format, or pointed to a member in an unsupported way.
    /// </summary>
    public class ExpressionNotSupportedException : ExpressionException
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNotSupportedException"/> class.
        /// </summary>
        public ExpressionNotSupportedException()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNotSupportedException"/> class.
        /// </summary>
        /// <param name="expression">The unsupported expression.</param>
        public ExpressionNotSupportedException(string expression)
            : base(string.Format(Resources.ExpressionNotSupportedExceptionMessage, expression))
        {
        }

        #endregion
    }
}
