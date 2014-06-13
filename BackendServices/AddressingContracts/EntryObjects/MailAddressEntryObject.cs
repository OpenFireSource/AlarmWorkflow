// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Net.Mail;
using System.Runtime.Serialization;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.BackendService.AddressingContracts.EntryObjects
{
    /// <summary>
    /// Represents a "Mail" entry in the address book.
    /// </summary>
    [DataContract()]
    public class MailAddressEntryObject
    {
        #region Constants

        /// <summary>
        /// Defines the type identifier for this entry object.
        /// </summary>
        public const string TypeId = "Mail";

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the mail address of this object.
        /// </summary>
        [DataMember()]
        public MailAddress Address { get; set; }
        /// <summary>
        /// Gets/sets the destined receipt type for mails sent to this address.
        /// </summary>
        [DataMember()]
        public ReceiptType Type { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Parses a specified address type into a <see cref="MailAddressEntryObject"/>-instance and uses "To" as recipient type.
        /// </summary>
        /// <param name="address">The e-mail address to parse.</param>
        /// <returns></returns>
        public static MailAddressEntryObject FromAddress(string address)
        {
            // TODO: Hardcoded string!
            return FromAddress(address, "To");
        }

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
