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
using System.Xml.Linq;
using AlarmWorkflow.Shared.Addressing.Extensibility;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Shared.Addressing
{
    /// <summary>
    /// Represents an address book, that contains the contact data of select persons and provides a means
    /// to retrieve this data to be used at run-time (e. g. in jobs).
    /// This class cannot be inherited.
    /// </summary>
    public sealed class AddressBook : IEnumerable<AddressBookEntry>, IStringSettingConvertible
    {
        #region Fields

        private List<IAddressProvider> _addressProviders;
        private List<IAddressFilter> _addressFilter;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of entries in this address book.
        /// </summary>
        public AddressBookEntryCollection Entries { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressBook"/> class.
        /// </summary>
        public AddressBook()
        {
            this.Entries = new AddressBookEntryCollection();

            // TODO: Make static
            // TODO: Make ETL an interface and use locator to ease testing!
            _addressProviders = new List<IAddressProvider>();
            _addressProviders.AddRange(ExportedTypeLibrary.ImportAll<IAddressProvider>());

            _addressFilter = new List<IAddressFilter>();
            AddSpecifiedAddressFilters();
        }

        #endregion

        #region Methods

        private void AddSpecifiedAddressFilters()
        {
            IList<string> exports = SettingsManager.Instance.GetSetting("Addressing", "FiltersConfiguration").GetValue<ExportConfiguration>().GetEnabledExports();
            foreach (var export in ExportedTypeLibrary
                .GetExports(typeof(IAddressFilter))
                .Where(j => exports.Contains(j.Attribute.Alias)))
            {
                _addressFilter.Add(export.CreateInstance<IAddressFilter>());
            }
        }

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
        /// <returns>An enumerable of tuples that contain both the entry and the custom data of this entry.</returns>
        public IEnumerable<Tuple<AddressBookEntry, TCustomData>> GetCustomObjects<TCustomData>(string type)
        {
            return GetCustomObjectsFiltered<TCustomData>(type, null);
        }

        /// <summary>
        /// Performs a query over all entries in this instance and returns all entries including their data items of the given type.
        /// Includes only entries that successfully match all the <see cref="IAddressFilter"/>s specified in the configuration.
        /// </summary>
        /// <typeparam name="TCustomData">The custom data to expect.</typeparam>
        /// <param name="type">The type to query.</param>
        /// <param name="operation">The <see cref="Operation"/> to use for filtering. Using null performs no filtering.</param>
        /// <returns>An enumerable of tuples that contain both the entry and the custom data of this entry.</returns>
        public IEnumerable<Tuple<AddressBookEntry, TCustomData>> GetCustomObjectsFiltered<TCustomData>(string type, Operation operation)
        {
            foreach (AddressBookEntry entry in Entries)
            {
                if (operation != null && _addressFilter.Any(fl => !fl.QueryAcceptEntry(operation, entry)))
                {
                    continue;
                }

                foreach (TCustomData data in entry.GetDataItems<TCustomData>(type))
                {
                    yield return Tuple.Create<AddressBookEntry, TCustomData>(entry, data);
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
            doc.Root.Add(new XAttribute("Version", 1));

            foreach (var entry in Entries)
            {
                XElement entryE = new XElement("Entry");
                if (string.IsNullOrWhiteSpace(entry.FirstName))
                {
                    entry.FirstName = Properties.Resources.UnknownNameSubstitute;
                }
                entryE.Add(new XAttribute("FirstName", entry.FirstName));
                if (string.IsNullOrWhiteSpace(entry.LastName))
                {
                    entry.LastName = Properties.Resources.UnknownNameSubstitute;
                }
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
