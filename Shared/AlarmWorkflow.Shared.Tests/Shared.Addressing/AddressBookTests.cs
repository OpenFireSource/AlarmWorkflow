using System.Linq;
using System.Net.Mail;
using AlarmWorkflow.Shared.Addressing;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Shared.Tests.Shared.Addressing
{
    /// <summary>
    /// Tests the functionality of the addressbook.
    /// </summary>
    [TestClass]
    public class AddressBookTests
    {
        #region Constants

        private static readonly string MailEntryObjectTypeIdentifier = "Mail";

        #endregion

        /// <summary>
        /// Creates an addressbook with one entry, one dataitem and converts it around.
        /// </summary>
        [TestMethod]
        public void SimpleConversionTest()
        {
            string sourceValue = null;

            // Create and convert
            {
                AddressBook addressBook = CreateAddressBookWithOneEntryAndOneDataItem(true);
                sourceValue = ((IStringSettingConvertible)addressBook).ConvertBack();
            }

            // Convert to address book and check values
            {
                AddressBook addressBook = new AddressBook();
                ((IStringSettingConvertible)addressBook).Convert(sourceValue);

                Assert.AreEqual(1, addressBook.Entries.Count);
                Assert.AreEqual(1, addressBook.Entries[0].Data.Count);
                Assert.AreEqual(MailEntryObjectTypeIdentifier, addressBook.Entries[0].Data[0].Identifier);
                Assert.AreEqual(true, addressBook.Entries[0].Data[0].IsEnabled);
                Assert.AreEqual(typeof(MailAddressEntryObject), addressBook.Entries[0].Data[0].Data.GetType());
            }
        }

        /// <summary>
        /// Tests that a data item that has "IsEnabled" set to "false" is not included in the result set.
        /// </summary>
        [TestMethod()]
        public void DontReturnDataItemIfIsEnabledIsFalse()
        {
            AddressBook addressBook = CreateAddressBookWithOneEntryAndOneDataItem(false);
            Assert.AreEqual(1, addressBook.Entries.Count);
            Assert.AreEqual(1, addressBook.Entries[0].Data.Count);

            var results = addressBook.GetCustomObjects<MailAddressEntryObject>(MailEntryObjectTypeIdentifier);
            Assert.AreEqual(0, results.Count());
        }

        private AddressBook CreateAddressBookWithOneEntryAndOneDataItem(bool dataItemIsEnabled)
        {
            AddressBook addressBook = new AddressBook();

            AddressBookEntry entry = new AddressBookEntry();
            entry.FirstName = "John";
            entry.LastName = "Doe";

            MailAddressEntryObject data = new MailAddressEntryObject() { Address = new MailAddress("john.doe@example.com") };
            entry.Data.Add(new EntryDataItem() { Data = data, Identifier = MailEntryObjectTypeIdentifier, IsEnabled = dataItemIsEnabled });

            addressBook.Entries.Add(entry);

            return addressBook;
        }
    }
}
