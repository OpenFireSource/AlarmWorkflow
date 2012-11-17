using System;
using System.Diagnostics;

namespace AlarmWorkflow.Parser.GenericParser.Misc
{
    /// <summary>
    /// Represents a string which can be controlled to be equal to another string if it is contained in it.
    /// </summary>
    [DebuggerDisplay("String = {String} (IsContained = {IsContained})")]
    class GenericParserString : IEquatable<string>
    {
        #region Properties

        public string String { get; set; }
        public bool IsContained { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParserString"/> class.
        /// </summary>
        public GenericParserString()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParserString"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="isContained"></param>
        public GenericParserString(string value, bool isContained)
            : this()
        {
            this.String = value;
            this.IsContained = isContained;
        }

        #endregion

        #region Methods

        public bool StartsWith(string line)
        {
            return line.StartsWith(this.String, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.String == null)
            {
                return "(String is null)";
            }
            return String;
        }

        #endregion

        #region IEquatable<string> Members

        /// <summary>
        /// Returns whether or not this string is equal to the other string.
        /// This string and the other string are considered equal if either
        /// the strings do match (ignoring the <see cref="P:IsContained"/>-property), or the
        /// <see cref="P:IsContained"/>-property is set to <c>true</c> and this string is only
        /// contained in the other string.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(string other)
        {
            if (other == null)
            {
                return this.String == null;
            }

            // UPPERCASE all strings to enable a better containment-check
            string left = this.String.ToUpperInvariant();
            string right = other.ToUpperInvariant();

            if (IsContained)
            {
                return right.Contains(left);
            }
            return right.Equals(left);
        }

        #endregion
    }
}
