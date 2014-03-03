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
using System.Runtime.Serialization;
using System.Xml.Linq;
using AlarmWorkflow.BackendService.AddressingContracts.AddressProviders;
using AlarmWorkflow.BackendService.AddressingContracts.Properties;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.BackendService.AddressingContracts
{
    /// <summary>
    /// Represents an address book, which stores information about people and can be used
    /// within jobs for personalized behavior.
    /// </summary>
    [DataContract()]
    public sealed class AddressBook : IStringSettingConvertible
    {
        #region Fields

        private static readonly List<IAddressProvider> _addressProviders;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the list of entries.
        /// </summary>
        [DataMember()]
        public IList<AddressBookEntry> Entries { get; set; }

        #endregion

        #region Constructors

        static AddressBook()
        {
            _addressProviders = new List<IAddressProvider>();
            _addressProviders.AddRange(ExportedTypeLibrary.ImportAll<IAddressProvider>());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressBook"/> class.
        /// </summary>
        public AddressBook()
            : base()
        {
            Entries = new List<AddressBookEntry>();
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

        #endregion

        #region IStringSettingConvertible Members

        void IStringSettingConvertible.Convert(string settingValue)
        {
            Logger.Instance.LogFormat(LogType.Debug, this, Resources.AddressBook_StartScanMessage);

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

            Logger.Instance.LogFormat(LogType.Debug, this, Resources.AddressBook_FinishScanMessage, Entries.Count);
        }

        string IStringSettingConvertible.ConvertBack()
        {
            XDocument doc = new XDocument();
            doc.Add(new XElement("AddressBook"));

            foreach (var entry in Entries)
            {
                XElement entryE = new XElement("Entry");
                if (string.IsNullOrWhiteSpace(entry.FirstName))
                {
                    entry.FirstName = Resources.UnknownNameSubstitute;
                }
                entryE.Add(new XAttribute("FirstName", entry.FirstName));
                if (string.IsNullOrWhiteSpace(entry.LastName))
                {
                    entry.LastName = Resources.UnknownNameSubstitute;
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
                        Logger.Instance.LogFormat(LogType.Error, this, Resources.ConvertBackErrorMessage, eo.Identifier);
                        Logger.Instance.LogException(this, ex);
                    }
                }

                doc.Root.Add(entryE);
            }

            return doc.ToString();
        }

        #endregion
    }
}
