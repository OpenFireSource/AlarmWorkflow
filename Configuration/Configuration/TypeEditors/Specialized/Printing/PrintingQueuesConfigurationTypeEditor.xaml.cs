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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;
using AlarmWorkflow.Windows.UIContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors.Specialized.Printing
{
    /// <summary>
    /// Interaction logic for PrintingQueuesConfigurationTypeEditor.xaml
    /// </summary>
    [Export("PrintingQueuesConfigurationTypeEditor", typeof(ITypeEditor))]
    public partial class PrintingQueuesConfigurationTypeEditor : UserControl, ITypeEditor
    {
        #region Fields

        private object _valueRaw;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintingQueuesConfigurationTypeEditor"/> class.
        /// </summary>
        public PrintingQueuesConfigurationTypeEditor()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void EditConfiguration_Click(object sender, RoutedEventArgs e)
        {
            // Manually create window
            Window wnd = new Window();
            wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            wnd.Owner = Application.Current.MainWindow;
            wnd.Title = Properties.Resources.PrintingQueuesConfigurationWindowTitle;
            wnd.Width = 600;
            wnd.Height = 400;
            wnd.Icon = BitmapFrame.Create(this.GetPackUri("Images/TypeEditors/PrintHS.png"));
            wnd.Closing += (a, b) =>
            {
                var result = MessageBox.Show(Properties.Resources.PrintingQueuesConfigurationWindowLeaveConfirmation_MSG, Properties.Resources.PrintingQueuesConfigurationWindowLeaveConfirmation_CAP, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Cancel)
                {
                    b.Cancel = true;
                    return;
                }

                wnd.DialogResult = (result == MessageBoxResult.Yes) ? true : false;
            };

            PrintingQueuesEditor editorctrl = new PrintingQueuesEditor();
            editorctrl.ValueWrapper = _valueRaw;
            wnd.Content = editorctrl;

            if (wnd.ShowDialog() == true)
            {
                _valueRaw = editorctrl.ValueWrapper;
            }
        }

        #endregion

        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value
        {
            get { return _valueRaw; }
            set { _valueRaw = value; }
        }

        /// <summary>
        /// Gets the visual element that is editing the value.
        /// </summary>
        public System.Windows.UIElement Visual
        {
            get { return this; }
        }

        void ITypeEditor.Initialize(string editorParameter)
        {
        }

        #endregion
    }
}