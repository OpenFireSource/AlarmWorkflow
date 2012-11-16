using System.Xml.Linq;
using AlarmWorkflow.Shared.Addressing;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Job.MailingJob
{
    [Export("MailingJobAddressProvider", typeof(IAddressProvider))]
    class MailingJobAddressProvider : IAddressProvider
    {
        #region IAddressProvider Members

        string IAddressProvider.AddressType
        {
            get { return "Mail"; }
        }

        object IAddressProvider.ParseXElement(XElement element)
        {
            string address = element.Value;
            string receiptType = element.TryGetAttributeValue("Type", MailingEntryObject.ReceiptType.To.ToString());

            return MailingEntryObject.FromAddress(address, receiptType);
        }

        #endregion
    }
}
