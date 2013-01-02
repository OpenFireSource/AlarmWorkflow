using System.Diagnostics;

namespace AlarmWorkflow.Parser.GenericParser.Parsing
{
    /// <summary>
    /// Representation of one token within a section.
    /// A token is a pair consisting of a key (the area keyword) and a value (the text behind the colon).
    /// </summary>
    [DebuggerDisplay("Identifier = {Identifier}; Value = {Value} (Orig: {OriginalValue})")]
    public sealed class AreaToken
    {
        #region Properties

        /// <summary>
        /// Gets/sets the token identifier of the area.
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// Gets/sets the value of the area.
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Gets/sets the raw line value from which this token originated.
        /// </summary>
        public string OriginalValue { get; set; }
        /// <summary>
        /// Gets/sets the index in the line where this token occurs.
        /// </summary>
        internal int Occurrence { get; set; }

        #endregion

        #region Constructors

        public AreaToken(string originalValue)
        {
            OriginalValue = originalValue;
            Occurrence = -1;
        }

        #endregion
    }
}
