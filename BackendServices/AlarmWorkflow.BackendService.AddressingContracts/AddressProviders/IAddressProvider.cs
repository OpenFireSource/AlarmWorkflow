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

namespace AlarmWorkflow.BackendService.AddressingContracts.AddressProviders
{
    /// <summary>
    /// Defines a means for a type that provides and handles custom addresses (such as Pager address, phone address etc.).
    /// </summary>
    interface IAddressProvider
    {
        /// <summary>
        /// Returns a string that identifies the type of the address this provider handles.
        /// </summary>
        string AddressType { get; }

        /// <summary>
        /// Converts the given <see cref="XElement"/> into a specific .Net object.
        /// </summary>
        /// <param name="element">The XElement to convert.</param>
        /// <returns>The specific .Net object.</returns>
        object Convert(XElement element);
        /// <summary>
        /// Converts the given specific .Net object from this instance into a serializable <see cref="XElement"/>.
        /// </summary>
        /// <param name="value">The specific .Net object to serialize.</param>
        /// <returns>The serializable <see cref="XElement"/> that contains the data from the specific object.</returns>
        XElement ConvertBack(object value);
    }
}