using System;
using System.Collections.Generic;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Shared.Addressing
{
    /// <summary>
    /// Implements the <see cref="IAddressBook"/>-interface.
    /// </summary>
    internal sealed class AddressBook : IAddressBook
    {
        #region Fields

        private List<IAddressProvider> _addressProviders;

        private List<AddressBookEntry> _entries;

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="AddressBook"/> class from being created.
        /// </summary>
        private AddressBook()
        {
            _entries = new List<AddressBookEntry>();

            _addressProviders = new List<IAddressProvider>();
            _addressProviders.AddRange(ExportedTypeLibrary.ImportAll<IAddressProvider>());
        }

        #endregion

        #region Methods

        internal static IAddressBook Parse(string xmlContent)
        {
            Logger.Instance.LogFormat(LogType.Debug, typeof(AddressBook), Properties.Resources.AddressBook_StartScanMessage);

            AddressBook addressBook = new AddressBook();

            // Parse document
            XDocument doc = XDocument.Parse(xmlContent);

            foreach (XElement entryE in doc.Root.Elements("Entry"))
            {
                AddressBookEntry entry = new AddressBookEntry();
                entry.Name = entryE.TryGetAttributeValue("Name", null);

                // Find all other custom attributes
                foreach (XElement customElementE in entryE.Elements())
                {
                    string providerType = customElementE.Name.LocalName;

                    IAddressProvider provider = addressBook.GetAddressProvider(providerType);
                    if (provider == null)
                    {
                        continue;
                    }

                    if (!IsEnabled(customElementE))
                    {
                        continue;
                    }

                    object customObject = provider.ParseXElement(customElementE);
                    if (customObject == null)
                    {
                        continue;
                    }

                    entry.CustomData[providerType] = customObject;
                }

                addressBook._entries.Add(entry);
            }

            Logger.Instance.LogFormat(LogType.Debug, typeof(AddressBook), Properties.Resources.AddressBook_FinishScanMessage, addressBook._entries.Count);

            return addressBook;
        }

        private IAddressProvider GetAddressProvider(string type)
        {
            return _addressProviders.Find(p => p.AddressType == type);
        }

        private static bool IsEnabled(XElement customElementE)
        {
            return customElementE.TryGetAttributeValue("IsEnabled", true);
        }

        #endregion

        #region IAddressBook Members

        IEnumerable<AddressBookEntry> IAddressBook.GetEntries()
        {
            return _entries;
        }

        IEnumerable<Tuple<AddressBookEntry, TCustomData>> IAddressBook.GetCustomObjects<TCustomData>(string type)
        {
            foreach (AddressBookEntry entry in _entries)
            {
                object customData = null;
                if (!entry.CustomData.TryGetValue(type, out customData))
                {
                    continue;
                }
                yield return Tuple.Create<AddressBookEntry, TCustomData>(entry, (TCustomData)customData);
            }
        }

        #endregion
    }
}
