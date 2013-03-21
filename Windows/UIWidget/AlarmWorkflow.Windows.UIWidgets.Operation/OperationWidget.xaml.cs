using System.Windows;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;

namespace AlarmWorkflow.Windows.UIWidgets.Operation
{
    [Export("OperationWidget", typeof(IUIWidget))]
    public partial class OperationWidget : IUIWidget
    {
        #region Fields

        private string _expressionOne;
        private string _expressionThree;
        private string _expressionTwo;

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
            _expressionOne = SettingsManager.Instance.GetSetting("OperationWidget", "LineOne").GetString();
            _expressionTwo = SettingsManager.Instance.GetSetting("OperationWidget", "LineTwo").GetString();
            _expressionThree = SettingsManager.Instance.GetSetting("OperationWidget", "LineThree").GetString();

            return true;
        }

        void IUIWidget.OnOperationChange(Shared.Core.Operation operation)
        {
            string lineOne = null;
            string lineTwo = null;
            string lineThree = null;

            if (!string.IsNullOrWhiteSpace(_expressionOne))
            {
                lineOne = operation != null ? operation.ToString(_expressionOne) : "(n/A)";
            }
            if (!string.IsNullOrWhiteSpace(_expressionTwo))
            {
                lineTwo = operation != null ? operation.ToString(_expressionTwo) : "(n/A)";
            }
            if (!string.IsNullOrWhiteSpace(_expressionThree))
            {
                lineThree = operation != null ? operation.ToString(_expressionThree) : "(n/A)";
            }

            LineOne.Inlines.Clear();
            LineThree.Inlines.Clear();
            LineThree.Inlines.Clear();
            LineTwo.Inlines.Clear();

            LineOne.Inlines.Add(Helper.Traverse(lineOne));
            LineTwo.Inlines.Add(Helper.Traverse(lineTwo));
            LineThree.Inlines.Add(Helper.Traverse(lineThree));
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