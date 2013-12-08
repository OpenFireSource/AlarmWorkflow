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

using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.BackendService.AddressingContracts.EntryObjects;
using AlarmWorkflow.BackendService.AddressingContracts.Properties;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.BackendService.AddressingContracts.AddressProviders
{
    [Export("MobilePhoneAddressProvider", typeof(IAddressProvider))]
    sealed class MobilePhoneAddressProvider : IAddressProvider
    {
        #region IAddressProvider Members

        string IAddressProvider.AddressType
        {
            get { return MobilePhoneEntryObject.TypeId; }
        }

        object IAddressProvider.Convert(XElement element)
        {
            string phoneNumber = element.Value.Trim();
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return null;
            }

            // Check for invalid chars in phone number
            if (phoneNumber.Any(c => char.IsLetter(c)))
            {
                Logger.Instance.LogFormat(LogType.Error, this, Resources.PhoneNumberContainsInvalidChars, phoneNumber);
                return null;
            }

            return new MobilePhoneEntryObject() { PhoneNumber = phoneNumber };
        }

        XElement IAddressProvider.ConvertBack(object value)
        {
            MobilePhoneEntryObject meo = (MobilePhoneEntryObject)value;

            XElement element = new XElement("dummy");
            element.Value = meo.PhoneNumber;
            return element;
        }

        #endregion
    }
}
