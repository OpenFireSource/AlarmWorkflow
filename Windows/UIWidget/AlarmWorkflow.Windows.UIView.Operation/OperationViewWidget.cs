using System.Windows;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;

namespace AlarmWorkflow.Windows.UIView.Operation
{
    [Export("OperationViewWidget", typeof(IUIWidget))]
    class OperationViewWidget : IUIWidget
    {
        private const string GUID = "b0b2c2c1-c5c6-4495-ab42-d50329babc32";
        private const string TITLE = "Operation-View";

        bool IUIWidget.Initialize()
        {
            uiElement = new TextBox();
            uiElement.TextWrapping = TextWrapping.WrapWithOverflow;
            return true;
        }

        void IUIWidget.OnOperationChange(Shared.Core.Operation operation)
        {
            if (operation != null)
            {
                uiElement.Text = operation.ToString();
            }
            else
            {
                uiElement.Text = "";
            }
        }

        private TextBox uiElement;

        UIElement IUIWidget.UIElement
        {
            get { return uiElement; }
        }

        string IUIWidget.ContentGuid
        {
            get { return GUID; }
        }

        string IUIWidget.Title
        {
            get { return TITLE; }
        }
    }
}