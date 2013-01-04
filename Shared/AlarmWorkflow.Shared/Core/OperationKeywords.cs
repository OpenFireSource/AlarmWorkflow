using System;
using System.Collections.Generic;

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

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String"/> that contains all set keywords separated by commas.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that contains all set keywords separated by commas.</returns>
        public override string ToString()
        {
            List<string> parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(Keyword))
            {
                parts.Add("Stichwort: " + Keyword);
            }
            if (!string.IsNullOrWhiteSpace(EmergencyKeyword))
            {
                parts.Add("Stichwort: " + EmergencyKeyword);
            }
            if (!string.IsNullOrWhiteSpace(B))
            {
                parts.Add("B: " + B);
            }
            if (!string.IsNullOrWhiteSpace(R))
            {
                parts.Add("R: " + R);
            }
            if (!string.IsNullOrWhiteSpace(S))
            {
                parts.Add("S: " + S);
            }
            if (!string.IsNullOrWhiteSpace(T))
            {
                parts.Add("T: " + T);
            }

            return string.Join(", ", parts);
        }

        #endregion
    }
}
