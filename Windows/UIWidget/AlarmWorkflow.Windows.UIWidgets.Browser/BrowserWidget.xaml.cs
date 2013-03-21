using System;
using System.Windows;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;

namespace AlarmWorkflow.Windows.UIWidgets.Browser
{
    /// <summary>
    /// Provides a Browserwidget
    /// </summary>
    [Export("BrowserWidget", typeof(IUIWidget))]
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
            _expressionUrl = SettingsManager.Instance.GetSetting("Browser", "URL").GetString();
            return !String.IsNullOrWhiteSpace(_expressionUrl);
        }

        void IUIWidget.OnOperationChange(Operation operation)
        {
            String url = ObjectFormatter.ToString(operation, _expressionUrl, ObjectFormatterOptions.RemoveNewlines);
            try
            {
                _webbrowser.Navigate(url);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
            }
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