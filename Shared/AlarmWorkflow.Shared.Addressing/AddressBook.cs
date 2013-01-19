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
    /// 
    /// </summary>
    public sealed class AddressBook : IEnumerable<AddressBookEntry>, IStringSettingConvertible
    {
        #region Fields

        private List<IAddressProvider> _addressProviders;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of entries in this address book.
        /// </summary>
        public AddressBookEntryCollection Entries { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="AddressBook"/> class from being created.
        /// </summary>
        public AddressBook()
        {
            this.Entries = new AddressBookEntryCollection();

            // TODO: Make static
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

        /// <summary>
        /// Performs a query over all entries in this instance and returns all entries including their data items of the given type.
        /// </summary>
        /// <typeparam name="TCustomData">The custom data to expect.</typeparam>
        /// <param name="type">The type to query.</param>
        /// <returns></returns>
        public IEnumerable<Tuple<AddressBookEntry, TCustomData>> GetCustomObjects<TCustomData>(string type)
        {
            // TODO: This should be a Lookup<AddressBookEntry, EntryDataItem> instead of returning a ton of tuples!
            foreach (AddressBookEntry entry in Entries)
            {
                IEnumerable<EntryDataItem> matching = entry.Data.Where(d => d.Identifier == type);
                foreach (EntryDataItem eo in matching)
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

            XDocument doc = XDocument.Parse(settingValue);

            foreach (XElement entryE in doc.Root.Elements("Entry"))
            {
                AddressBookEntry entry = new AddressBookEntry();
                entry.FirstName = entryE.TryGetAttributeValue("FirstName", null);
                entry.LastName = entryE.TryGetAttributeValue("LastName", null);

                // Find all other custom attributes
                foreach (XElement customElementE in entryE.Elements())
                {
                    string providerType = customElementE.Name.LocalName;

                    IAddressProvider provider = GetAddressProvider(providerType);
                    if (provider == null)
                    {
                        continue;
                    }

                    object customObject = provider.Convert(customElementE);
                    if (customObject == null)
                    {
                        continue;
                    }

                    EntryDataItem eo = new EntryDataItem();
                    eo.IsEnabled = IsEnabled(customElementE);
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
                entryE.Add(new XAttribute("FirstName", entry.FirstName));
                entryE.Add(new XAttribute("LastName", entry.LastName));

                foreach (EntryDataItem eo in entry.Data)
                {
                    IAddressProvider provider = GetAddressProvider(eo.Identifier);
                    if (provider == null)
                    {
                        continue;
                    }

                    try
                    {

                        XElement eoE = provider.ConvertBack(eo.Data);
                        eoE.Name = eo.Identifier;
                        eoE.Add(new XAttribute("IsEnabled", eo.IsEnabled));

                        entryE.Add(eoE);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.ConvertBackErrorMessage, eo.Identifier);
                        Logger.Instance.LogException(this, ex);
                    }
                }

                doc.Root.Add(entryE);
            }

            return doc.ToString();
        }

        #endregion

        #region IEnumerable<AddressBookEntry> Members

        /// <summary>
        /// Returns an enumerator that enumerates over all entries in this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<AddressBookEntry> GetEnumerator()
        {
            return this.Entries.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
