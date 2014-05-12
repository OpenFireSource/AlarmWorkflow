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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    class GroupedSectionViewModel : ViewModelBase
    {
        #region Properties

        public ObservableCollection<GroupedSectionViewModel> Children { get; private set; }
        public SectionViewModel Section { get; private set; }

        public bool IsSelected { get; set; }
        public string Identifier { get; set; }
        public string Header { get; set; }
        public int Order { get; set; }

        #endregion

        #region Constructors

        internal GroupedSectionViewModel(SectionViewModel section)
        {
            Children = new ObservableCollection<GroupedSectionViewModel>();

            ICollectionView view = CollectionViewSource.GetDefaultView(Children);
            view.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Ascending));

            if (section != null)
            {
                Section = section;

                Identifier = Section.Identifier;
                Header = Section.DisplayText;
                Order = section.Order;
            }
        }

        #endregion
    }
}