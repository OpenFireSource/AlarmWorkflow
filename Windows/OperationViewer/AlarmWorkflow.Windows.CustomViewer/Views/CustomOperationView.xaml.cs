using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;
using AlarmWorkflow.Windows.UIContracts.Extensibility;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;

namespace AlarmWorkflow.Windows.CustomViewer.Views
{
    /// <summary>
    ///     Interaction logic for CustomOperationViewer.xaml
    /// </summary>
    [Export("CustomOperationViewer", typeof(IOperationViewer))]
    public partial class CustomOperationView : UserControl, IOperationViewer
    {
        private readonly string LayoutFile = Path.Combine(Utilities.GetWorkingDirectory(), "Config", "CustomOperationViewer.layout");
        private readonly WidgetManager _WidgetManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomOperationView"/> class.
        /// </summary>
        public CustomOperationView()
        {
            InitializeComponent();
            _WidgetManager = new WidgetManager();
            List<ILayoutPanelElement> elements = _WidgetManager.InitializeViews();
            foreach (ILayoutPanelElement layoutPanelElement in elements)
            {
                rootLayout.RootPanel.Children.Add(layoutPanelElement);
            }

            var serializer = new XmlLayoutSerializer(dockingManager);
            if (File.Exists(LayoutFile))
            {
                serializer.Deserialize(LayoutFile);
            }

        }

        void IOperationViewer.OnNewOperation(Operation operation)
        {
        }

        void IOperationViewer.OnOperationChanged(Operation operation)
        {
            foreach (IUIWidget uiWidget in _WidgetManager.Widgets)
            {
                uiWidget.OnOperationChange(operation);
            }
        }

        FrameworkElement IOperationViewer.Visual
        {
            get { return this; }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            var serializer = new XmlLayoutSerializer(dockingManager);
            serializer.Serialize(new XmlTextWriter(LayoutFile, Encoding.UTF8));
        }
    }
}