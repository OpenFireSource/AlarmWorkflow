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
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Shared.Specialized.Printing;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors.Specialized.Printing
{
    /// <summary>
    /// Interaction logic for PrintingQueuesEditor.xaml
    /// </summary>
    public partial class PrintingQueuesEditor : UserControl
    {
        #region Fields

        private PrintingQueuesEditorViewModel _viewModel;

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
                StringSettingConvertibleTools.ConvertBack(_viewModel.EditWrapper, out value);
                return value;
            }
            set
            {
                PrintingQueuesConfiguration pqc = StringSettingConvertibleTools.ConvertFromSetting<PrintingQueuesConfiguration>(value);
                _viewModel.EditWrapper = pqc;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintingQueuesEditor"/> class.
        /// </summary>
        public PrintingQueuesEditor()
        {
            InitializeComponent();

            _viewModel = new PrintingQueuesEditorViewModel();
            this.DataContext = _viewModel;
        }

        #endregion
    }
}