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
            string lineOne = operation != null ? operation.ToString(SettingsManager.Instance.GetSetting("OperationWidget", "LineOne").GetString()) : "(n/A)";
            string lineTwo = operation != null ? operation.ToString(SettingsManager.Instance.GetSetting("OperationWidget", "LineTwo").GetString()) : "(n/A)";
            string lineThree = operation != null ? operation.ToString(SettingsManager.Instance.GetSetting("OperationWidget", "LineThree").GetString()) : "(n/A)";
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