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
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    class CategoryViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets/sets the name of the category.
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Gets a list of all setting item VMs.
        /// </summary>
        public List<SettingItemViewModel> SettingItems { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryViewModel"/> class.
        /// </summary>
        public CategoryViewModel()
        {
            SettingItems = new List<SettingItemViewModel>();
        }

        #endregion
    }
}