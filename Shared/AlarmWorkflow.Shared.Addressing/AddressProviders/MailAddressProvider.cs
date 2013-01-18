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

        object IAddressProvider.ParseXElement(XElement element)
        {
            string address = element.Value;
            string receiptType = element.TryGetAttributeValue("Type", MailAddressEntryObject.ReceiptType.To.ToString());

            return MailAddressEntryObject.FromAddress(address, receiptType);
        }

        #endregion
    }
}
