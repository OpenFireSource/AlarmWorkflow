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

using System.Windows.Controls;
using AlarmWorkflow.BackendService.AddressingContracts;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.Configuration.AddressBookEditor.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor.Views
{
    /// <summary>
    /// Interaction logic for AddressBookEditorControl.xaml
    /// </summary>
    public partial class AddressBookEditorControl : UserControl
    {
        #region Fields

        private AddressBookViewModel _viewModel;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current value by creating an addressbook out of all current entries, and sets the value from the settings manager.
        /// </summary>
        internal object ValueWrapper
        {
            get
            {
                string value = null;
                StringSettingConvertibleTools.ConvertBack(_viewModel.AddressBookEditWrapper, out value);
                return value;
            }
            set
            {
                AddressBook ab = StringSettingConvertibleTools.ConvertFromSetting<AddressBook>(value);

                _viewModel = new AddressBookViewModel(ab);
                this.DataContext = _viewModel;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressBookEditorControl"/> class.
        /// </summary>
        public AddressBookEditorControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}