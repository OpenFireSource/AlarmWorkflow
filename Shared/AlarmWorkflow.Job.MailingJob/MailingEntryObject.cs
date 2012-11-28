using System;
using System.Net.Mail;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.MailingJob
{
    public class MailingEntryObject
    {
        public MailAddress Address { get; private set; }
        public ReceiptType Type { get; private set; }

        public static MailingEntryObject FromAddress(string address, string receiptType)
        {
            MailingEntryObject returnValue = new MailingEntryObject();
            returnValue.Address = TryParseMailAddress(address);
            if (returnValue.Address == null)
            {
                return null;
            }

            ReceiptType receiptTypeEnum = ReceiptType.To;
            if (!Enum.TryParse<ReceiptType>(receiptType, true, out receiptTypeEnum))
            {
                return null;
            }

            returnValue.Type = receiptTypeEnum;

            return returnValue;
        }

        private static MailAddress TryParseMailAddress(string address)
        {
            try
            {
                return Helpers.ParseAddress(address);
            }
            catch (Exception)
            {
                // Parsing failed
                Logger.Instance.LogFormat(LogType.Warning, null, "The address '{0}' failed to parse. This is usually an indication that the E-Mail address is invalid formatted.", address);
            }
            return null;
        }

        public enum ReceiptType
        {
            To,
            CC,
            Bcc,
        }
    }
}
