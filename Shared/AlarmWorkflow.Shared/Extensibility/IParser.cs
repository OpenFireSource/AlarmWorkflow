using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Extensibility
{
    /// <summary>
    /// Defines a parser which parses a string[].
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Parses the contents of the given lines into an <see cref="Operation"/>-representation.
        /// </summary>
        /// <param name="lines">The line contents of the analysed file.</param>
        /// <returns>The operation-instance that contains the data from the fax.</returns>
        Operation Parse(string[] lines);
    }
}
