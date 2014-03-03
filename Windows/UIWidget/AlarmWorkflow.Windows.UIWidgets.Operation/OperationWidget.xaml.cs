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
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.SettingsContracts;
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

        private string _expressionLineOne;
        private string _expressionLineTwo;
        private string _expressionLineThree;

        #endregion

        #region Constructor

        public OperationWidget()
        {
            InitializeComponent();
        }

        #endregion

        #region IUIWidget Members

        bool IUIWidget.Initialize()
        {
            using (var service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(new SettingsServiceCallback()))
            {
                _expressionLineOne = service.Instance.GetSetting(SettingKeys.LineOne).GetValue<string>();
                _expressionLineTwo = service.Instance.GetSetting(SettingKeys.LineTwo).GetValue<string>();
                _expressionLineThree = service.Instance.GetSetting(SettingKeys.LineThree).GetValue<string>();
            }
            return true;
        }

        void IUIWidget.OnOperationChange(Shared.Core.Operation operation)
        {
            string lineOne = null;
            if (!string.IsNullOrWhiteSpace(_expressionLineOne))
            {
                lineOne = operation != null ? operation.ToString(_expressionLineOne) : "(n/A)";
            }
            string lineTwo = null;
            if (!string.IsNullOrWhiteSpace(_expressionLineTwo))
            {
                lineTwo = operation != null ? operation.ToString(_expressionLineTwo) : "(n/A)";
            }
            string lineThree = null;
            if (!string.IsNullOrWhiteSpace(_expressionLineThree))
            {
                lineThree = operation != null ? operation.ToString(_expressionLineThree) : "(n/A)";
            }
            LineOne.Inlines.Clear();
            LineTwo.Inlines.Clear();
            LineThree.Inlines.Clear();

            LineOne.Inlines.Add(Helper.Execute(lineOne));
            LineTwo.Inlines.Add(Helper.Execute(lineTwo));
            LineThree.Inlines.Add(Helper.Execute(lineThree));
        }


        UIElement IUIWidget.UIElement
        {
            get { return this; }
        }

        string IUIWidget.ContentGuid
        {
            get { return "b0b2c2c1-c5c6-4495-ab42-d50329babc32"; }
        }

        string IUIWidget.Title
        {
            get { return "Alarmtext"; }
        }

        #endregion
    }
}