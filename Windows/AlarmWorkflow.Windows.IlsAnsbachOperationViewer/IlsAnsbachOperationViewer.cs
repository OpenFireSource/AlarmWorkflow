using AlarmWorkflow.Parser.IlsAnsbachParser;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UI.Extensibility;

namespace AlarmWorkflow.Windows.IlsAnsbachOperationViewer
{
    [Export("IlsAnsbachOperationViewer", typeof(IOperationViewer))]
    [OperationViewer(typeof(IlsAnsbachParser))]
    class IlsAnsbachOperationViewer : IOperationViewer
    {
        #region IOperationViewer Members

        System.Windows.FrameworkElement IOperationViewer.CreateTemplate()
        {
            return new IlsAnsbachOperationView();
        }

        #endregion
    }
}
