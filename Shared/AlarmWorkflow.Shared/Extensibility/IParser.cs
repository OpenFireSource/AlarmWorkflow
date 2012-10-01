using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Extensibility
{
    /// <summary>
    /// Defines a parser which parses an incoming alarmfax.
    /// </summary>
    public interface IParser : IExtensionObject
    {
        /// <summary>
        /// Parses the contents of an analysed alarmfax into an <see cref="Operation"/>-representation.
        /// </summary>
        /// <param name="lines">The line contents of the analysed file.</param>
        /// <returns>A filled einsatz object.</returns>
        Operation Parse(string[] lines);
    }
}
