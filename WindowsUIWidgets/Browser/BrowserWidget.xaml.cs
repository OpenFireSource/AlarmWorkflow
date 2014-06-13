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
using AlarmWorkflow.Shared.ObjectExpressions;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;

namespace AlarmWorkflow.Windows.UIWidgets.Browser
{
    /// <summary>
    /// Provides a Browserwidget
    /// </summary>
    [Export("BrowserWidget", typeof(IUIWidget))]
    [Information(DisplayName = "ExportUIWidgetDisplayName", Description = "ExportUIWidgetDescription")]
    public partial class BrowserWidget : IUIWidget
    {
        #region Fields

        private string _expressionUrl;

        #endregion

        #region Constructors

        /// <summary>
        /// Provides a Browserwidget
        /// </summary>
        public BrowserWidget()
        {
            InitializeComponent();
        }

        #endregion

        #region IUWidget Members

        bool IUIWidget.Initialize()
        {
            using (var service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(new SettingsServiceCallback()))
            {
                _expressionUrl = service.Instance.GetSetting(SettingKeys.Url).GetValue<string>();
            }
            return !string.IsNullOrWhiteSpace(_expressionUrl);
        }

        void IUIWidget.OnOperationChange(Operation operation)
        {
            string url = ObjectFormatter.ToString(operation, _expressionUrl, ObjectFormatterOptions.RemoveNewlines);
            _webbrowser.Navigate(url);
        }

        UIElement IUIWidget.UIElement
        {
            get { return this; }
        }

        string IUIWidget.ContentGuid
        {
            get { return "CDF33773-BE09-41A4-896F-173180495147"; }
        }

        string IUIWidget.Title
        {
            get { return "Browser"; }
        }

        #endregion
    }
}