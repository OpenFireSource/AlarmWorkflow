using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UI.Extensibility;

namespace AlarmWorkflow.Windows.UI.Views
{
    /// <summary>
    /// Interaction logic for DefaultOperationView.xaml
    /// </summary>
    [Export("DefaultOperationView", typeof(IOperationViewer))]
    public partial class DefaultOperationView : UserControl, IOperationViewer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultOperationView"/> class.
        /// </summary>
        public DefaultOperationView()
        {
            InitializeComponent();
        }

        #endregion

        #region IOperationViewer Members

        System.Windows.FrameworkElement IOperationViewer.Create()
        {
            return this;
        }

        void IOperationViewer.OnOperationChanged(Shared.Core.Operation operation)
        {

        }

        #endregion
    }
}
