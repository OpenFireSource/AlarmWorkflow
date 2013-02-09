using System.Windows;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;

namespace AlarmWorkflow.Windows.UIWidgets.Operation
{
    [Export("OperationWidget", typeof (IUIWidget))]
    public partial class OperationWidget : IUIWidget
    {
        public OperationWidget()
        {
            InitializeComponent();
        }

        #region IUIWidget Members

        bool IUIWidget.Initialize()
        {
            return true;
        }

        void IUIWidget.OnOperationChange(Shared.Core.Operation operation)
        {
            string settingLineOne = SettingsManager.Instance.GetSetting("OperationWidget", "LineOne").GetString();
            string lineOne = null;
            if (!string.IsNullOrWhiteSpace(settingLineOne))
            {
                lineOne = operation != null ? operation.ToString(settingLineOne) : "(n/A)";
            }
            string settingLineTwo = SettingsManager.Instance.GetSetting("OperationWidget", "LineTwo").GetString();
            string lineTwo = null;
            if (!string.IsNullOrWhiteSpace(settingLineTwo))
            {
                lineTwo = operation != null ? operation.ToString(settingLineTwo) : "(n/A)";
            }

            string settingLineThree = SettingsManager.Instance.GetSetting("OperationWidget", "LineThree").GetString();
            string lineThree = null;
            if (!string.IsNullOrWhiteSpace(settingLineOne))
            {
                lineThree = operation != null ? operation.ToString(settingLineThree) : "(n/A)";
            }
            LineOne.Inlines.Clear();
            LineOne.Inlines.Add(Helper.Traverse(lineOne));
            LineTwo.Inlines.Clear();
            LineTwo.Inlines.Add(Helper.Traverse(lineTwo));
            LineThree.Inlines.Clear();
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
            get { return "Operation-View"; }
        }

        #endregion
    }
}