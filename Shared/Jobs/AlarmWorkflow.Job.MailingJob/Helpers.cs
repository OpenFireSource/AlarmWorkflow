using System;
using System.Net.Mail;

namespace AlarmWorkflow.Job.MailingJob
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
