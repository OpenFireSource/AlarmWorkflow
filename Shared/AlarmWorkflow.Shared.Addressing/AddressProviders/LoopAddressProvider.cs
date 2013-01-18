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

        object IAddressProvider.ParseXElement(XElement element)
        {
            string loop = element.Value;

            LoopEntryObject leo = new LoopEntryObject();
            leo.Loop = loop;
            return leo;
        }

        #endregion
    }
}
