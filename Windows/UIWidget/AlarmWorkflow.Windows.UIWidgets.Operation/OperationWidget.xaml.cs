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
            _expressionLineOne = SettingsManager.Instance.GetSetting("OperationWidget", "LineOne").GetString();
            _expressionLineTwo = SettingsManager.Instance.GetSetting("OperationWidget", "LineTwo").GetString();
            _expressionLineThree = SettingsManager.Instance.GetSetting("OperationWidget", "LineThree").GetString();
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