using System;

namespace AlarmWorkflow.Shared.ObjectExpressions
{
    /// <summary>
    /// Specifies the options that can be used to alter the way the <see cref="T:ObjectFormatter" /> formats an object.
    /// </summary>
    [Flags()]
    public enum ObjectFormatterOptions
    {
        /// <summary>
        /// Uses no options. This is the default value.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Removes newline characters from the format string, resulting in one long line.
        /// </summary>
        RemoveNewlines = 1,
        /// <summary>
        /// Inserts a question mark '[?]' for any value that is <c>null</c>.
        /// If this option is not specified, nothing is inserted (empty).
        /// </summary>
        InsertQuestionMarksForNullValues = 2,
    }
}
