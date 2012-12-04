using System.Collections.Generic;

namespace AlarmWorkflow.Job.SmsJob
{
    /// <summary>
    /// Exposes the interface that can be used to extend the SMS-Job with custom providers.
    /// </summary>
    public interface ISmsProvider
    {
        /// <summary>
        /// Sends an SMS via the provider.
        /// </summary>
        /// <param name="userName">Name of the user (for login).</param>
        /// <param name="password">The password (for login).</param>
        /// <param name="phoneNumbers">The phone numbers to send the message to.</param>
        /// <param name="messageText">The message text. This text is always capped to 160 signs including ellipsis if too long.</param>
        void Send(string userName, string password, IEnumerable<string> phoneNumbers, string messageText);
    }
}
