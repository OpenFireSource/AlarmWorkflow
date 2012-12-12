using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;
using AlarmWorkflow.Windows.UIContracts.Extensibility;
using AvalonDock.Layout;

namespace AlarmWorkflow.Windows.CustomViewer.Views
{
    /// <summary>
    ///     Interaction logic for CustomOperationViewer.xaml
    /// </summary>
    [Export("CustomOperationViewer", typeof(IOperationViewer))]
    public partial class CustomOperationView : UserControl, IOperationViewer
    {
        public CustomOperationView()
        {
            InitializeComponent();
            var viewManager = new ViewManager();
            List<ILayoutPanelElement> elements = viewManager.InitializeViews();
            foreach (ILayoutPanelElement layoutPanelElement in elements)
            {
                rootLayout.RootPanel.Children.Add(layoutPanelElement);
            }
        }

        void IOperationViewer.OnNewOperation(Operation operation)
        {
        }

        void IOperationViewer.OnOperationChanged(Operation operation)
        {
        }

        FrameworkElement IOperationViewer.Visual
        {
            get { return this; }
        }
    }
}