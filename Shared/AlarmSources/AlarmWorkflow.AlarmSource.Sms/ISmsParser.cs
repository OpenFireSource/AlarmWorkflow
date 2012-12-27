using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.AlarmSource.Sms
{
    /// <summary>
    /// Defines a mechanism for a parser that parses the SMS contents of an SMS sent by a specific call center.
    /// </summary>
    public interface ISmsParser
    {
        /// <summary>
        /// Parses the specified SMS text and constructs a usable <see cref="Operation"/>-object.
        /// </summary>
        /// <param name="smsText">The SMS text to parse into an <see cref="Operation"/>.</param>
        /// <returns>A <see cref="Operation"/>-object from the SMS text.</returns>
        Operation Parse(string smsText);
    }
}
