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
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;

namespace AlarmWorkflow.Windows.UIWidgets.Resources
{
    /// <summary>
    /// 
    /// </summary>
    [Export("ResourcesWidget", typeof(IUIWidget))]
    [Information(DisplayName = "ExportUIWidgetDisplayName", Description = "ExportUIWidgetDescription")]
    public partial class ResourcesWidget : IUIWidget
    {
        #region Fields

        private ViewModel _model;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesWidget"/> class.
        /// </summary>
        public ResourcesWidget()
        {
            InitializeComponent();

            _model = new ViewModel();
            DataContext = _model;
        }

        #endregion

        #region IUIWidget Members

        bool IUIWidget.Initialize()
        {
            return true;
        }

        void IUIWidget.OnOperationChange(Operation operation)
        {
            _model.OperationChanged(operation);
        }

        UIElement IUIWidget.UIElement
        {
            get { return this; }
        }

        string IUIWidget.ContentGuid
        {
            get { return "E4E4AA8C-A00B-4087-AF06-9D81143DB23D"; }
        }

        string IUIWidget.Title
        {
            get { return "Fahrzeuge"; }
        }

        #endregion
    }
}
