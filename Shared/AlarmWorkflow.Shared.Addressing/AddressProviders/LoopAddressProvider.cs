using System.Xml.Linq;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Addressing.AddressProviders
{
    [Export("LoopAddressProvider", typeof(IAddressProvider))]
    sealed class LoopAddressProvider : IAddressProvider
    {
        #region IAddressProvider Members

        string IAddressProvider.AddressType
        {
            get { return "Loop"; }
        }

        object IAddressProvider.Convert(XElement element)
        {
            string loop = element.Value;

            LoopEntryObject leo = new LoopEntryObject();
            leo.Loop = loop;
            return leo;
        }

        XElement IAddressProvider.ConvertBack(object value)
        {
            LoopEntryObject leo = (LoopEntryObject)value;

            XElement element = new XElement("dummy");
            element.Value = leo.Loop;
            return element;
        }

        #endregion
    }
}
