using System.Net.Mail;

namespace AlarmWorkflow.Job.eAlarm
{
    static class Helpers
    {
        internal static MailAddress ParseAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return null;
            }
            return new MailAddress(address);
        }
    }
}
