
namespace AlarmWorkflow.Shared.ObjectExpressions
{
    /// <summary>
    /// Represents the results of a user-defined property resolving operation.
    /// </summary>
    public class ResolveExpressionResult
    {
        #region Properties

        /// <summary>
        /// Gets/sets the status of resolving the property.
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Gets/sets the resolved value. The value is only considered if <see cref="Success"/> evaluates to <c>true</c>.
        /// </summary>
        public object ResolvedValue { get; set; }

        /// <summary>
        /// Determines whether or not the <see cref="ResolvedValue"/> actually has a value after all.
        /// </summary>
        public bool ResolvedValueHasValue
        {
            get { return ResolvedValue != null; }
        }

        /// <summary>
        /// Returns a new instance of <see cref="ResolveExpressionResult"/> which indicates failure.
        /// </summary>
        public static ResolveExpressionResult Fail
        {
            get { return new ResolveExpressionResult(); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveExpressionResult"/> class.
        /// </summary>
        public ResolveExpressionResult()
        {

        }

        #endregion
    }
}
