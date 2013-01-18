using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Shared.Addressing.AddressProviders
{
    [Export("MobilePhoneAddressProvider", typeof(IAddressProvider))]
    sealed class MobilePhoneAddressProvider : IAddressProvider
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
                Logger.Instance.LogFormat(LogType.Error, this, "The phone number '{0}' contains invalid chars! Make sure it does only contain digits (0-9)!", phoneNumber);
                return null;
            }

            return new MobilePhoneEntryObject() { PhoneNumber = phoneNumber };
        }

        #endregion
    }
}
