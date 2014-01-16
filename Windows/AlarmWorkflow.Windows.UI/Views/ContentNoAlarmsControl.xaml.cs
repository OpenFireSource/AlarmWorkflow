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

namespace AlarmWorkflow.Windows.UI.Views
{
    /// <summary>
    /// Interaction logic for ContentNoAlarmsControl.xaml
    /// </summary>
    public partial class ContentNoAlarmsControl : UserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNoAlarmsControl"/> class.
        /// </summary>
        public ContentNoAlarmsControl()
        {
            InitializeComponent();

            Helper.SetDisplayModeRequired(false);

            if (!App.GetApp().StartRoutine)
            {
                App.GetApp().ExtensionManager.RunIdleUIJobs();
            }
            else
            {
                App.GetApp().StartRoutine = false;
            }
        }

        #endregion
    }
}