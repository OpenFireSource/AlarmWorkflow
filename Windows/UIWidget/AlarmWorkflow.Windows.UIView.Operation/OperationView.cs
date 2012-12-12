using System.Windows;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;


namespace AlarmWorkflow.Windows.UIView.Operation
{
    [Export("OperationView", typeof(IUIWidget))]
    public class OperationView : IUIWidget
    {
        private const string GUID = "b0b2c2c1-c5c6-4495-ab42-d50329babc32";
        bool IUIWidget.Initialize()
        {
            uiElement = new TextBox();
            return true;
        }

        void IUIWidget.OnOperationChange(Shared.Core.Operation operation)
        {
           
        }

        private UIElement uiElement;
        UIElement IUIWidget.UIElement
        {
            get { return uiElement; }
        }

        string IUIWidget.ContentGuid {
            get { return GUID; }
        }
    }
}
