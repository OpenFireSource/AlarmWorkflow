using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Addressing;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.SmsJob
{
    [Export("MobilePhoneAddressProvider", typeof(IAddressProvider))]
    class MobilePhoneAddressProvider : IAddressProvider
    {
        #region IAddressProvider Members

        string IAddressProvider.AddressType
        {
            get { return "MobilePhone"; }
        }

        object IAddressProvider.ParseXElement(XElement element)
        {
            string phoneNumber = element.Value.Trim();
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return null;
            }

            // Check for invalid chars in phone number
            if (phoneNumber.Any(c => char.IsLetter(c)))
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.PhoneNumberContainsInvalidCharsMessage, phoneNumber);
                return null;
            }

            return new MobilePhoneEntryObject() { PhoneNumber = phoneNumber };
        }

        #endregion
    }
}
