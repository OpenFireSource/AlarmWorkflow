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
            string lineOne = FormatLine(_expressionLineOne, operation);
            string lineTwo = FormatLine(_expressionLineTwo, operation);
            string lineThree = FormatLine(_expressionLineThree, operation);
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

        #region Methods

        private string FormatLine(string expression, Shared.Core.Operation operation)
        {
            if (!string.IsNullOrWhiteSpace(expression))
            {
                try
                {
                    return operation != null ? operation.ToString(expression) : "(n/A)";
                }
                catch (AssertionFailedException)
                {
                    // This exception may occure if the format of the value is broken or other problems with the format exist...
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        #endregion

    }
}