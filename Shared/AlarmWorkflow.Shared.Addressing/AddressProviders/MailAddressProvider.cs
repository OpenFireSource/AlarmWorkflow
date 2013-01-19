using System.Xml.Linq;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Addressing.AddressProviders
{
    [Export("MailAddressProvider", typeof(IAddressProvider))]
    sealed class MailAddressProvider : IAddressProvider
    {
        #region IAddressProvider Members

        string IAddressProvider.AddressType
        {
            get { return "Mail"; }
        }

        object IAddressProvider.Convert(XElement element)
        {
            string address = element.Value;
            string receiptType = element.TryGetAttributeValue("Type", MailAddressEntryObject.ReceiptType.To.ToString());

            return MailAddressEntryObject.FromAddress(address, receiptType);
        }

        XElement IAddressProvider.ConvertBack(object value)
        {
            MailAddressEntryObject meo = (MailAddressEntryObject)value;

            XElement element = new XElement("dummy");
            element.Add(new XAttribute("Type", meo.Type.ToString()));
            element.Value = meo.Address.Address;
            return element;
        }

        #endregion
    }
}
