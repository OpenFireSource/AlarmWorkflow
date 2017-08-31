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
using System.Windows;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;

namespace AlarmWorkflow.Windows.UIWidgets.Operation
{
    /// <summary>
    /// 
    /// </summary>
    [Export("OperationWidget", typeof(IUIWidget))]
    [Information(DisplayName = "ExportUIWidgetDisplayName", Description = "ExportUIWidgetDescription")]
    public partial class OperationWidget : IUIWidget
    {

        #region Fields

        private readonly ViewModel _dataContext;

        #endregion

        #region Constructor

        public OperationWidget()
        {
            InitializeComponent();
            _dataContext = new ViewModel();
            DataContext = _dataContext;
        }

        #endregion

        #region IUIWidget Members

        bool IUIWidget.Initialize()
        {
            return _dataContext.Initialize();
        }

        void IUIWidget.OnOperationChange(Shared.Core.Operation operation)
        {
            _dataContext.OnOperationChange(operation);
        }

        void IUIWidget.Close()
        {
            _dataContext.Dispose();
        }


        UIElement IUIWidget.UIElement => this;

        string IUIWidget.ContentGuid => "b0b2c2c1-c5c6-4495-ab42-d50329babc32";

        string IUIWidget.Title => "Alarmtext";

        #endregion
    }
}