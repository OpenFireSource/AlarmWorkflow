using System.Xml.Linq;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Addressing.AddressProviders
{
    [Export("PushAddressProvider", typeof(IAddressProvider))]
    sealed class PushAddressProvider : IAddressProvider
    {
        #region IAddressProvider Members

        string IAddressProvider.AddressType
        {
            get { return "Push"; }
        }

        object IAddressProvider.Convert(XElement element)
        {
            string consumer = element.TryGetAttributeValue("Consumer", null);
            string recApiKey = element.Value;

            PushEntryObject geo = new PushEntryObject();
            geo.Consumer = consumer;
            geo.RecipientApiKey = recApiKey;
            return geo;
        }

        XElement IAddressProvider.ConvertBack(object value)
        {
            PushEntryObject geo = (PushEntryObject)value;

            XElement element = new XElement("dummy");
            element.Add(new XAttribute("Consumer", geo.Consumer));
            element.Value = geo.RecipientApiKey;
            return element;
        }

        #endregion
    }
}
