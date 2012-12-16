using System;
using System.Text;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Contains the keywords ("stichwörter") for an operation.
    /// </summary>
    [Serializable()]
    public sealed class OperationKeywords
    {
        #region Properties

        /// <summary>
        /// Gets/sets the "Stichwort" (generic keyword), direct or equivalent.
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// Gets/sets the B/R/S/T/etc. keyword for sources that don't distinguish between them.
        /// </summary>
        public string EmergencyKeyword { get; set; }
        /// <summary>
        /// Gets/sets the "Stichwort B" (specific keyword), direct or equivalent.
        /// </summary>
        public string B { get; set; }
        /// <summary>
        /// Gets/sets the "Stichwort R" (specific keyword), direct or equivalent.
        /// </summary>
        public string R { get; set; }
        /// <summary>
        /// Gets/sets the "Stichwort S" (specific keyword), direct or equivalent.
        /// </summary>
        public string S { get; set; }
        /// <summary>
        /// Gets/sets the "Stichwort T" (specific keyword), direct or equivalent.
        /// </summary>
        public string T { get; set; }

        #endregion
    }
}
