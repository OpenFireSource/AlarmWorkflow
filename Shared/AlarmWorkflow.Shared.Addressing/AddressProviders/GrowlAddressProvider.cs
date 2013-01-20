using System.Xml.Linq;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Addressing.AddressProviders
{
    [Export("GrowlAddressProvider", typeof(IAddressProvider))]
    sealed class GrowlAddressProvider : IAddressProvider
    {
        #region IAddressProvider Members

        string IAddressProvider.AddressType
        {
            get { return "Growl"; }
        }

        object IAddressProvider.Convert(XElement element)
        {
            string consumer = element.TryGetAttributeValue("Consumer", null);
            string recApiKey = element.Value;

            GrowlEntryObject geo = new GrowlEntryObject();
            geo.Consumer = consumer;
            geo.RecipientApiKey = recApiKey;
            return geo;
        }

        XElement IAddressProvider.ConvertBack(object value)
        {
            GrowlEntryObject geo = (GrowlEntryObject)value;

            XElement element = new XElement("dummy");
            element.Add(new XAttribute("Consumer", geo.Consumer));
            element.Value = geo.RecipientApiKey;
            return element;
        }

        #endregion
    }
}
