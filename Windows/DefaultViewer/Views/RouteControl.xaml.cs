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
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.DefaultViewer.Views
{
    /// <summary>
    /// Interaction logic for RouteControl.xaml
    /// </summary>
    public partial class RouteControl : UserControl
    {
        #region Properties

        /// <summary>
        /// Gets/sets the <see cref="T:Operation"/>-instance to get the route image from.
        /// </summary>
        public Operation Operation
        {
            get { return (Operation)GetValue(OperationProperty); }
            set { SetValue(OperationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Operation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OperationProperty = DependencyProperty.Register("Operation", typeof(Operation), typeof(RouteControl), new UIPropertyMetadata(null));

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteControl"/> class.
        /// </summary>
        public RouteControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}