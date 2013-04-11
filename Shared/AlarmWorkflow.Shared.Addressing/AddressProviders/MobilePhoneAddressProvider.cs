using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Addressing.Properties;
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

        object IAddressProvider.Convert(XElement element)
        {
            string phoneNumber = element.Value.Trim();
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return null;
            }

            // Check for invalid chars in phone number
            if (phoneNumber.Any(c => char.IsLetter(c)))
            {
                Logger.Instance.LogFormat(LogType.Error, this, Resources.PhoneNumberContainsInvalidChars, phoneNumber);
                return null;
            }

            return new MobilePhoneEntryObject() { PhoneNumber = phoneNumber };
        }

        XElement IAddressProvider.ConvertBack(object value)
        {
            MobilePhoneEntryObject meo = (MobilePhoneEntryObject)value;

            XElement element = new XElement("dummy");
            element.Value = meo.PhoneNumber;
            return element;
        }

        #endregion
    }
}
