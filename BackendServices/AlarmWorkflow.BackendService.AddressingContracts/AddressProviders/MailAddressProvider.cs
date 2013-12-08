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
    [Export("MailAddressProvider", typeof(IAddressProvider))]
    sealed class MailAddressProvider : IAddressProvider
    {
        #region IAddressProvider Members

        string IAddressProvider.AddressType
        {
            get { return MailAddressEntryObject.TypeId; }
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
