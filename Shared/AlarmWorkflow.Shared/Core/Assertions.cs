using System;
using System.Collections.Generic;
using System.Text;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Defines methods that can be used to assert cases; may be used as extensions.
    /// </summary>
    public static class Assertions
    {
        /// <summary>
        /// Asserts that a given variable is not null, and raises an exception if it is.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="name">The name of the value.</param>
        /// <exception cref="AssertionFailedException">Raised if the <paramref name="value"/> is <c>null</c>.</exception>
        public static void AssertNotNull(object value, string name)
        {
            AssertNotNull(value, name, "Value is null!");
        }
        /// <summary>
        /// Asserts that a given variable is not null, and raises an exception if it is.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="name">The name of the value.</param>
        /// <param name="message">The message to display.</param>
        /// <exception cref="AssertionFailedException">Raised if the <paramref name="value"/> is <c>null</c>.</exception>
        public static void AssertNotNull(object value, string name, string message)
        {
            if (value == null)
            {
                throw new AssertionFailedException(message, "NotNull", new string[] { name });
            }
        }
        /// <summary>
        /// Asserts that a given string is not empty, and raises an exception if it is.
        /// </summary>
        /// <param name="value">The string-value to check.</param>
        /// <param name="name">The name of the value.</param>
        /// <exception cref="AssertionFailedException">Raised if the <paramref name="value"/> is <c>null</c> or empty.</exception>
        public static void AssertNotEmpty(string value, string name)
        {
            AssertNotEmpty(value, name, "Value is empty!");
        }
        /// <summary>
        /// Asserts that a given string is not empty, and raises an exception if it is.
        /// </summary>
        /// <param name="value">The string-value to check.</param>
        /// <param name="name">The name of the value.</param>
        /// <param name="message">The message to display.</param>
        /// <exception cref="AssertionFailedException">Raised if the <paramref name="value"/> is <c>null</c> or empty.</exception>
        public static void AssertNotEmpty(string value, string name, string message)
        {
            AssertNotNull(name, "name");
            if (string.IsNullOrEmpty(value))
            {
                throw new AssertionFailedException(message, "NotEmpty", new string[] { name });
            }
        }

        /// <summary>
        /// Represents the exception that occurs if an assertion has failed.
        /// </summary>
        public sealed class AssertionFailedException : Exception
        {
            /// <summary>
            /// Gets the name of the assertion.
            /// </summary>
            /// <value>The name of the assertion.</value>
            public string AssertionName { get; private set; }
            /// <summary>
            /// Gets the affected parameter names.
            /// </summary>
            public string[] AffectedParameterNames { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="AssertionFailedException"/> class.
            /// </summary>
            /// <param name="assertionName">Name of the assertion.</param>
            /// <param name="affectedParameterNames">The affected parameter names.</param>
            public AssertionFailedException(string assertionName, string[] affectedParameterNames)
                : this("An assertion has failed", assertionName, affectedParameterNames)
            {

            }

            /// <summary>
            /// Initializes a new instance of the <see cref="AssertionFailedException"/> class.
            /// </summary>
            /// <param name="message">The message to display if the assertion has failed.</param>
            /// <param name="assertionName">Name of the assertion.</param>
            /// <param name="affectedParameterNames">The affected parameter names.</param>
            public AssertionFailedException(string message, string assertionName, string[] affectedParameterNames)
                : base(message)
            {
                this.AssertionName = assertionName;
                this.AffectedParameterNames = affectedParameterNames;
            }
        }
    }
}
