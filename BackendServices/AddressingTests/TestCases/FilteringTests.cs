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

using System;
using System.Collections.Generic;
using System.Net.Mail;
using AlarmWorkflow.BackendService.Addressing.AddressFilter;
using AlarmWorkflow.BackendService.AddressingContracts;
using AlarmWorkflow.BackendService.AddressingContracts.EntryObjects;
using AlarmWorkflow.BackendService.AddressingContracts.Extensibility;
using AlarmWorkflow.Shared.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Shared.Tests.Shared.Addressing
{
    [TestClass]
    public class FilteringTests
    {
        [TestMethod]
        public void ByLoopAddressFilterTest()
        {
            IAddressFilter filter = new ByLoopAddressFilter();

            Operation operation = new Operation();
            operation.Loops.Add("123");
            operation.Loops.Add("456");
            operation.Loops.Add("789");

            AddressBookEntry entry = new AddressBookEntry();
            entry.FirstName = "John";
            entry.LastName = "Doe";
            entry.Data.Add(new EntryDataItem() { Identifier = LoopEntryObject.TypeId, IsEnabled = true, Data = new LoopEntryObject() { Loop = "123" } });

            Assert.IsTrue(filter.QueryAcceptEntry(operation, entry));

            entry.Data.Clear();
            Assert.IsFalse(filter.QueryAcceptEntry(operation, entry));

            entry.Data.Add(new EntryDataItem() { Identifier = LoopEntryObject.TypeId, IsEnabled = true, Data = new LoopEntryObject() { Loop = "nada" } });
            Assert.IsFalse(filter.QueryAcceptEntry(operation, entry));
        }
    }
}
