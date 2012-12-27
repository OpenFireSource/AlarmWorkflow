using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.AlarmSource.Fax
{
    /// <summary>
    /// Defines a parser which parses an incoming alarmfax.
    /// </summary>
    public interface IFaxParser
    {
        /// <summary>
        /// Parses the contents of an analysed alarmfax into an <see cref="Operation"/>-representation.
        /// </summary>
        /// <param name="lines">The line contents of the analysed file.</param>
        /// <returns>The operation-instance that contains the data from the fax.</returns>
        Operation Parse(string[] lines);
    }
}
