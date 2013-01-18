using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Shared.Addressing
{
    /// <summary>
    /// Implements the <see cref="IAddressBook"/>-interface.
    /// </summary>
    public sealed class AddressBook : IStringSettingConvertible
    {
        #region Fields

        private List<IAddressProvider> _addressProviders;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of entries in this address book.
        /// </summary>
        public EntryCollection Entries { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="AddressBook"/> class from being created.
        /// </summary>
        private AddressBook()
        {
            this.Entries = new EntryCollection();

            _addressProviders = new List<IAddressProvider>();
            _addressProviders.AddRange(ExportedTypeLibrary.ImportAll<IAddressProvider>());
        }

        #endregion

        #region Methods

        private IAddressProvider GetAddressProvider(string type)
        {
            return _addressProviders.Find(p => p.AddressType == type);
        }

        private static bool IsEnabled(XElement customElementE)
        {
            return customElementE.TryGetAttributeValue("IsEnabled", true);
        }

        public IEnumerable<AddressBookEntry> GetEntries()
        {
            return this.Entries;
        }

        public IEnumerable<Tuple<AddressBookEntry, TCustomData>> GetCustomObjects<TCustomData>(string type)
        {
            foreach (AddressBookEntry entry in Entries)
            {
                IEnumerable<EntryObject> matching = entry.Data.Where(d => d.Identifier == type);
                foreach (EntryObject eo in matching)
                {
                    yield return Tuple.Create<AddressBookEntry, TCustomData>(entry, (TCustomData)eo.Data);
                }
            }
        }

        #endregion

        #region IStringSettingConvertible Members

        void IStringSettingConvertible.Convert(string settingValue)
        {
            Logger.Instance.LogFormat(LogType.Debug, this, Properties.Resources.AddressBook_StartScanMessage);

            // Parse document
            XDocument doc = XDocument.Parse(settingValue);

            foreach (XElement entryE in doc.Root.Elements("Entry"))
            {
                AddressBookEntry entry = new AddressBookEntry();
                entry.Name = entryE.TryGetAttributeValue("Name", null);

                // Find all other custom attributes
                foreach (XElement customElementE in entryE.Elements())
                {
                    string providerType = customElementE.Name.LocalName;

                    IAddressProvider provider = GetAddressProvider(providerType);
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

                    EntryObject eo = new EntryObject();
                    eo.Identifier = providerType;
                    eo.Data = customObject;
                    entry.Data.Add(eo);
                }

                Entries.Add(entry);
            }

            Logger.Instance.LogFormat(LogType.Debug, this, Properties.Resources.AddressBook_FinishScanMessage, Entries.Count);
        }

        string IStringSettingConvertible.ConvertBack()
        {
            XDocument doc = new XDocument();
            doc.Add(new XElement("AddressBook"));

            foreach (var entry in Entries)
            {
                XElement entryE = new XElement("Entry");
                entryE.Add(new XAttribute("Name", entry.Name));

                foreach (var data in entry.Data)
                {
                    XElement dataE = new XElement(data.Identifier);
                    // TODO: IAddressProvider must convert back!

                    entryE.Add(dataE);
                }

                doc.Root.Add(entryE);
            }

            return doc.ToString();
        }

        #endregion
    }
}
