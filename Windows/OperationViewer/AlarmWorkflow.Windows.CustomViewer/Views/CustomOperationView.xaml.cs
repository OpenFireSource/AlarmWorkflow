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
    [Export("CustomOperationViewer", typeof (IOperationViewer))]
    public partial class CustomOperationView : UserControl, IOperationViewer
    {
        private readonly ViewManager _ViewManager;

        public CustomOperationView()
        {
            InitializeComponent();
            _ViewManager = new ViewManager();
            List<ILayoutPanelElement> elements = _ViewManager.InitializeViews();
            foreach (ILayoutPanelElement layoutPanelElement in elements)
            {
                rootLayout.RootPanel.Children.Add(layoutPanelElement);
            }
            var serializer = new XmlLayoutSerializer(dockingManager);
            if (File.Exists("test.xml"))
                serializer.Deserialize("test.xml");
         
        }

        void IOperationViewer.OnNewOperation(Operation operation)
        {
        }

        void IOperationViewer.OnOperationChanged(Operation operation)
        {
            foreach (IUIWidget uiWidget in _ViewManager.Widgets)
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
            serializer.Serialize(new XmlTextWriter("test.xml", Encoding.UTF8));
        }
    }
}