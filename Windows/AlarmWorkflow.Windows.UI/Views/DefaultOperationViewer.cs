using System.Windows;
using AlarmWorkflow.Windows.UI.Extensibility;

namespace AlarmWorkflow.Windows.UI.Views
{
    class DefaultOperationViewer : IOperationViewer
    {
        #region IOperationViewer Members

        FrameworkElement IOperationViewer.CreateTemplate()
        {
            return new DefaultOperationView();
        }

        #endregion
    }
}
