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
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.BackendService.AddressingContracts;
using AlarmWorkflow.BackendService.AddressingContracts.Extensibility;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.Addressing
{
    class AddressingServiceInternal : InternalServiceBase, IAddressingServiceInternal
    {
        #region Fields

        private AddressBook _addressBook;
        private List<IAddressFilter> _addressFilter;

        #endregion

        #region Properties

        private ISettingsServiceInternal SettingsService
        {
            get { return ServiceProvider.GetService<ISettingsServiceInternal>(); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressingServiceInternal"/> class.
        /// </summary>
        public AddressingServiceInternal()
            : base()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Overridden to perform initialization logic on startup.
        /// </summary>
        public override void OnStart()
        {
            base.OnStart();

            _addressBook = SettingsService.GetSetting("Addressing", "AddressBook").GetValue<AddressBook>();

            _addressFilter = new List<IAddressFilter>();
            AddSpecifiedAddressFilters();
        }

        private void AddSpecifiedAddressFilters()
        {
            IList<string> exports = SettingsService.GetSetting("Addressing", "FiltersConfiguration").GetValue<ExportConfiguration>().GetEnabledExports();
            foreach (var export in ExportedTypeLibrary.GetExports(typeof(IAddressFilter)).Where(j => exports.Contains(j.Attribute.Alias)))
            {
                _addressFilter.Add(export.CreateInstance<IAddressFilter>());
            }
        }

        private IEnumerable<Tuple<AddressBookEntry, TCustomData>> GetCustomObjectsFiltered<TCustomData>(string type, Operation operation)
        {
            foreach (AddressBookEntry entry in _addressBook.Entries)
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

        #region IAddressingServiceInternal Members

        IList<AddressBookEntry> IAddressingServiceInternal.GetAllEntries()
        {
            return _addressBook.Entries;
        }

        IEnumerable<Tuple<AddressBookEntry, TCustomData>> IAddressingServiceInternal.GetCustomObjects<TCustomData>(string type)
        {
            return GetCustomObjectsFiltered<TCustomData>(type, null);
        }

        IEnumerable<Tuple<AddressBookEntry, TCustomData>> IAddressingServiceInternal.GetCustomObjectsFiltered<TCustomData>(string type, Operation operation)
        {
            return GetCustomObjectsFiltered<TCustomData>(type, operation);
        }

        #endregion
    }
}
