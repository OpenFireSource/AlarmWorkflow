using System;
using System.Net.Mail;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.MailingJob
{
    class MailingEntryObject
    {
        internal MailAddress Address { get; private set; }
        internal ReceiptType Type { get; private set; }

        internal static MailingEntryObject FromAddress(string address, string receiptType)
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

        internal enum ReceiptType
        {
            To,
            CC,
            Bcc,
        }
    }
}
