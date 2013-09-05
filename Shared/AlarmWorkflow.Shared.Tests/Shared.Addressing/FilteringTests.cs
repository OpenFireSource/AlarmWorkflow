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
using System.Linq;
using System.Net.Mail;
using AlarmWorkflow.Shared.Addressing;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Addressing.Extensibility;
using AlarmWorkflow.Shared.Addressing.Extensibility.AddressFilter;
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
            // Arrange

            IAddressFilter filter = new ByLoopAddressFilter();

            Operation operation = new Operation();
            operation.Loops.Add("123");
            operation.Loops.Add("456");
            operation.Loops.Add("789");

            AddressBookEntry entry = new AddressBookEntry();
            entry.FirstName = "John";
            entry.LastName = "Doe";
            entry.Data.Add(new EntryDataItem() { Identifier = LoopEntryObject.TypeId, IsEnabled = true, Data = new LoopEntryObject() { Loop = "123" } });

            // Act
            // Assert
            Assert.IsTrue(filter.QueryAcceptEntry(operation, entry));

            // Arrange
            entry.Data.Clear();
            // Act
            // Assert
            Assert.IsFalse(filter.QueryAcceptEntry(operation, entry));

            // Arrange
            entry.Data.Add(new EntryDataItem() { Identifier = LoopEntryObject.TypeId, IsEnabled = true, Data = new LoopEntryObject() { Loop = "nada" } });
            // Act
            // Assert
            Assert.IsFalse(filter.QueryAcceptEntry(operation, entry));
        }

        [TestMethod]
        public void GetCustomObjectsFilteredTestBasic()
        {
            // Arrange
            AddressBookEntry entry = new AddressBookEntry();
            entry.FirstName = "John";
            entry.LastName = "Doe";
            entry.Data.Add(new EntryDataItem() { Identifier = LoopEntryObject.TypeId, IsEnabled = true, Data = new LoopEntryObject() { Loop = "123" } });
            entry.Data.Add(new EntryDataItem() { Identifier = MailAddressEntryObject.TypeId, IsEnabled = true, Data = new MailAddressEntryObject() { Address = new MailAddress("a@b.com"), Type = MailAddressEntryObject.ReceiptType.CC } });
            entry.Data.Add(new EntryDataItem() { Identifier = MobilePhoneEntryObject.TypeId, IsEnabled = true, Data = new MobilePhoneEntryObject() { PhoneNumber = "01234/567890" } });
            entry.Data.Add(new EntryDataItem() { Identifier = PushEntryObject.TypeId, IsEnabled = true, Data = new PushEntryObject() { Consumer = "OFS", RecipientApiKey = "1234567890ABCDEF" } });

            AddressBook_Accessor book = new AddressBook_Accessor();
            book.Entries.Add(entry);

            // Act
            {
                var result = book.GetCustomObjects<LoopEntryObject>(LoopEntryObject.TypeId).ToList();
                // Assert
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("123", result[0].Item2.Loop);
            }

            // Act
            {
                var result = book.GetCustomObjects<MailAddressEntryObject>(MailAddressEntryObject.TypeId).ToList();
                // Assert
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("a@b.com", result[0].Item2.Address.Address);
                Assert.AreEqual(MailAddressEntryObject.ReceiptType.CC, result[0].Item2.Type);
            }

            // Act
            {
                var result = book.GetCustomObjects<MobilePhoneEntryObject>(MobilePhoneEntryObject.TypeId).ToList();
                // Assert
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("01234/567890", result[0].Item2.PhoneNumber);
            }

            // Act
            {
                var result = book.GetCustomObjects<PushEntryObject>(PushEntryObject.TypeId).ToList();
                // Assert
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("OFS", result[0].Item2.Consumer);
                Assert.AreEqual("1234567890ABCDEF", result[0].Item2.RecipientApiKey);
            }
        }

        [TestMethod]
        public void GetCustomObjectsFilteredTestBasicWithDisabledData()
        {
            // Arrange
            AddressBookEntry entry = new AddressBookEntry();
            entry.FirstName = "John";
            entry.LastName = "Doe";
            entry.Data.Add(new EntryDataItem() { Identifier = LoopEntryObject.TypeId, IsEnabled = true, Data = new LoopEntryObject() { Loop = "123" } });
            entry.Data.Add(new EntryDataItem() { Identifier = LoopEntryObject.TypeId, IsEnabled = false, Data = new LoopEntryObject() { Loop = "456" } });
            entry.Data.Add(new EntryDataItem() { Identifier = LoopEntryObject.TypeId, IsEnabled = true, Data = new LoopEntryObject() { Loop = "789" } });

            AddressBook_Accessor book = new AddressBook_Accessor();
            book.Entries.Add(entry);

            // Act
            var result = book.GetCustomObjects<LoopEntryObject>(LoopEntryObject.TypeId).ToList();
            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("123", result[0].Item2.Loop);
            Assert.AreEqual("789", result[1].Item2.Loop);
        }

        [TestMethod]
        public void GetCustomObjectsFilteredTest()
        {
            // Arrange
            Operation operation = new Operation();
            operation.Loops.Add("123");
            operation.Loops.Add("456");
            operation.Loops.Add("789");

            AddressBookEntry entry = new AddressBookEntry();
            entry.FirstName = "John";
            entry.LastName = "Doe";
            entry.Data.Add(new EntryDataItem() { Identifier = LoopEntryObject.TypeId, IsEnabled = true, Data = new LoopEntryObject() { Loop = "123" } });
            // This entry is disabled and shall be ignored altogether.
            entry.Data.Add(new EntryDataItem() { Identifier = LoopEntryObject.TypeId, IsEnabled = false, Data = new LoopEntryObject() { Loop = "999" } });

            AddressBook_Accessor book = new AddressBook_Accessor();
            book._addressFilter.Add(new ByLoopAddressFilter());
            book.Entries.Add(entry);

            List<Tuple<AddressBookEntry, LoopEntryObject>> result = null;

            // -------- Scenario: Filter with operation set and entry data contains loop from operation.
            // Act
            result = book.GetCustomObjectsFiltered<LoopEntryObject>(LoopEntryObject.TypeId, operation).ToList();
            // Assert
            Assert.AreEqual(1, result.Count);

            // -------- Scenario: Filter with operation set and entry data not containing any loop from operation.
            // Arrange
            entry.Data.Clear();
            entry.Data.Add(new EntryDataItem() { Identifier = LoopEntryObject.TypeId, IsEnabled = true, Data = new LoopEntryObject() { Loop = "nada" } });
            // Act
            result = book.GetCustomObjectsFiltered<LoopEntryObject>(LoopEntryObject.TypeId, operation).ToList();
            // Assert
            Assert.AreEqual(0, result.Count);

            // -------- Scenario: Filter with operation not set and entry data not containing any loop from operation.
            // Act
            result = book.GetCustomObjectsFiltered<LoopEntryObject>(LoopEntryObject.TypeId, null).ToList();
            // Assert
            Assert.AreEqual(1, result.Count);

            // -------- Scenario: Filter with operation set and address book not containing any entries.
            // Arrange
            book.Entries.Clear();
            // Act
            result = book.GetCustomObjectsFiltered<LoopEntryObject>(LoopEntryObject.TypeId, operation).ToList();
            // Assert
            Assert.AreEqual(0, result.Count);
        }
    }
}
