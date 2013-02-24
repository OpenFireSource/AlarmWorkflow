using System;

namespace AlarmWorkflow.Windows.ConfigurationContracts
{
    /// <summary>
    /// Represents an error that occurred while retrieving the value from a type editor.
    /// </summary>
    [Serializable()]
    public class ValueException : Exception
    {
        #region Properties

        /// <summary>
        /// Gets/sets the hint, telling the user what to do to fix the error.
        /// </summary>
        public string Hint { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="ValueException"/> class from being created.
        /// </summary>
        private ValueException()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueException"/> class.
        /// </summary>
        /// <param name="message">The message for the user.</param>
        public ValueException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueException"/> class.
        /// </summary>
        /// <param name="message">The message for the user.</param>
        /// <param name="hint">The hint for the user.</param>
        public ValueException(string message, string hint)
            : this(message)
        {
            this.Hint = hint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueException"/> class.
        /// </summary>
        /// <param name="message">The message for the user.</param>
        /// <param name="hint">The hint for the user. Must not be null.</param>
        /// <param name="hintArgs">The arguments to use </param>
        public ValueException(string message, string hint, params object[] hintArgs)
            : this(message, string.Format(hint, hintArgs))
        {

        }

        #endregion
    }
}
