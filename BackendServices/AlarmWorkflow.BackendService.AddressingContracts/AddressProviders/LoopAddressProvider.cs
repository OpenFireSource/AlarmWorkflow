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
    [Export("LoopAddressProvider", typeof(IAddressProvider))]
    sealed class LoopAddressProvider : IAddressProvider
    {
        #region IAddressProvider Members

        string IAddressProvider.AddressType
        {
            get { return LoopEntryObject.TypeId; }
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
