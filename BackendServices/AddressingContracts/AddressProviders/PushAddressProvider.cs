// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System.Xml.Linq;
using AlarmWorkflow.BackendService.AddressingContracts.EntryObjects;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.AddressingContracts.AddressProviders
{
    [Export("PushAddressProvider", typeof(IAddressProvider))]
    sealed class PushAddressProvider : IAddressProvider
    {
        #region IAddressProvider Members

        string IAddressProvider.AddressType
        {
            get { return PushEntryObject.TypeId; }
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
