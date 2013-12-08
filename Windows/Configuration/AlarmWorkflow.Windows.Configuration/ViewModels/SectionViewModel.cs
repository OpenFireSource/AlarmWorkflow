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

using System.Collections.Generic;
using System.Diagnostics;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    [DebuggerDisplay("Section = {Identifier}")]
    class SectionViewModel : ViewModelBase
    {
        #region Fields

        private IdentifierInfo _identifier;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of this section.
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// Gets the text to display in the UI.
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (_identifier == null || string.IsNullOrWhiteSpace(_identifier.DisplayText))
                {
                    return Identifier;
                }
                return _identifier.DisplayText;
            }
        }
        /// <summary>
        /// Gets a list of all setting item VMs.
        /// </summary>
        public List<CategoryViewModel> CategoryItems { get; private set; }
        /// <summary>
        /// Gets the Order of this section.
        /// </summary>
        public int Order
        {
            get { return _identifier.Order; }
        }

        #endregion

        #region Constructors

        private SectionViewModel()
        {
            CategoryItems = new List<CategoryViewModel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionViewModel"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public SectionViewModel(IdentifierInfo identifier)
            : this()
        {
            Assertions.AssertNotNull(identifier, "identifier");

            Identifier = identifier.Name;
            _identifier = identifier;
        }

        #endregion

        #region Methods

        internal void Add(SettingInfo info, SettingItem setting)
        {
            string categoryText = GetCategoryText(info.Category);

            CategoryViewModel cvm = CategoryItems.Find(c => c.Category == categoryText);
            if (cvm == null)
            {
                cvm = new CategoryViewModel();
                cvm.Category = categoryText;
                CategoryItems.Add(cvm);
            }

            cvm.SettingItems.Add(new SettingItemViewModel(info, setting));
        }

        private string GetCategoryText(string categoryText)
        {
            return string.IsNullOrWhiteSpace(categoryText) ? "(Sonstige)" : categoryText;
        }

        #endregion

    }
}