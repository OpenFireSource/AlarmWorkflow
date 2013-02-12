using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.CustomViewer.Extensibility;
using AlarmWorkflow.Windows.UIContracts.Extensibility;
using AvalonDock;
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
        private readonly string LayoutFile = Path.Combine(Utilities.GetLocalAppDataFolderPath(), "CustomOperationViewer.layout");
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
            List<String> fileIDs = new List<String>();
            var tempSerializer = new XmlLayoutSerializer(new DockingManager());
            tempSerializer.LayoutSerializationCallback += (sender, args) => fileIDs.Add(args.Model.ContentId);
            if (File.Exists(LayoutFile))
            {
                tempSerializer.Deserialize(LayoutFile);
            }
            bool everthingFound = _WidgetManager.Widgets.Select(uiWidget => fileIDs.Any(fileID => fileID.ToLower().Equals(uiWidget.ContentGuid.ToLower()))).All(found => found);
            if (everthingFound)
            {
                var serializer = new XmlLayoutSerializer(dockingManager);
                if (File.Exists(LayoutFile))
                {
                    serializer.Deserialize(LayoutFile);
                }

            }
        }

        void IOperationViewer.OnNewOperation(Operation operation)
        {
            foreach (IUIWidget uiWidget in _WidgetManager.Widgets)
            {
                uiWidget.OnOperationChange(operation);
            }
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
            XmlTextWriter writer = new XmlTextWriter(LayoutFile, Encoding.UTF8);
            serializer.Serialize(writer);
            writer.Flush();
            writer.Close();

        }
    }
}