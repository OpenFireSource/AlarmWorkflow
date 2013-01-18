using System;
using System.Net.Mail;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Shared.Addressing.EntryObjects
{
    /// <summary>
    /// Represents a "Mail" entry in the address book.
    /// </summary>
    public class MailAddressEntryObject
    {
        #region Properties

        /// <summary>
        /// Gets the mail address of this object.
        /// </summary>
        public MailAddress Address { get; private set; }
        /// <summary>
        /// Gets the destined receipt type for mails sent to this address.
        /// </summary>
        public ReceiptType Type { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Parses a specified address and recipient type into a <see cref="MailAddressEntryObject"/>-instance.
        /// </summary>
        /// <param name="address">The e-mail address to parse.</param>
        /// <param name="receiptType">The receipt type.</param>
        /// <returns></returns>
        public static MailAddressEntryObject FromAddress(string address, string receiptType)
        {
            MailAddressEntryObject returnValue = new MailAddressEntryObject();
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
                return ParseAddress(address);
            }
            catch (Exception)
            {
                // Parsing failed
                Logger.Instance.LogFormat(LogType.Warning, null, "The address '{0}' failed to parse. This is usually an indication that the E-Mail address is invalid formatted.", address);
            }
            return null;
        }

        private static MailAddress ParseAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return null;
            }
            return new MailAddress(address);
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Specifies the receipt type of a mail (To, Cc or Bcc).
        /// </summary>
        public enum ReceiptType
        {
            /// <summary>
            /// Direct addressee (To).
            /// </summary>
            To = 0,
            /// <summary>
            /// Carbon copy (Cc).
            /// </summary>
            CC,
            /// <summary>
            /// Blind carbon copy (Bcc).
            /// </summary>
            Bcc,
        }

        #endregion

    }
}
