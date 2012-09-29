
namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// ReplaceString struct defines a toupl of two Strings. Searching for an string an replace it with an new one.
    /// </summary>
    public struct ReplaceString
    {
        #region Properties

        /// <summary>
        /// Gets or sets the old string.
        /// </summary>
        public string OldString { get; set; }
        /// <summary>
        /// Gets or sets the new string.
        /// </summary>
        public string NewString { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="str1">The first ReplaceString.</param>
        /// <param name="str2">The second ReplaceString.</param>
        /// <returns>Indicates if both are equal.</returns>
        public static bool operator ==(ReplaceString str1, ReplaceString str2)
        {
            return str1.Equals(str2);
        }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="str1">The first ReplaceString.</param>
        /// <param name="str2">The second ReplaceString.</param>
        /// <returns>Indicates if both are not equal.</returns>
        public static bool operator !=(ReplaceString str1, ReplaceString str2)
        {
            return !str1.Equals(str2);
        }

        /// <summary>
        /// Compares a ReplaceString struct with a object.
        /// </summary>
        /// <param name="obj">The object to compare the ReplaceString with.</param>
        /// <returns>Indicates if both are equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ReplaceString)
            {
                return this.Equals((ReplaceString)obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Compares two ReplaceString structs.
        /// </summary>
        /// <param name="str">The ReplaceString to compare with.</param>
        /// <returns>Indicates if both are equal.</returns>
        public bool Equals(ReplaceString str)
        {
            if (str.NewString == this.NewString && str.OldString == this.OldString)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Overrides the getHashCode methode. 
        /// </summary>
        /// <returns>Returns the hash code.</returns>
        public override int GetHashCode()
        {
            return (this.OldString + this.NewString).GetHashCode();
        }

        #endregion
    }
}
