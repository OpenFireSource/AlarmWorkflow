using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
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
    [Information(DisplayName = "ExportCowDisplayName", Description = "ExportCowDescription")]
    public partial class CustomOperationView : IOperationViewer
    {
        private readonly string _layoutFile = Path.Combine(Utilities.GetLocalAppDataFolderPath(), "CustomOperationViewer.layout");
        private readonly WidgetManager _widgetManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomOperationView"/> class.
        /// </summary>
        public CustomOperationView()
        {
            InitializeComponent();
            _widgetManager = new WidgetManager();
            List<ILayoutPanelElement> elements = _widgetManager.InitializeViews();
            foreach (ILayoutPanelElement layoutPanelElement in elements)
            {
                rootLayout.RootPanel.Children.Add(layoutPanelElement);
            }
            List<String> fileIDs = new List<String>();
            var tempSerializer = new XmlLayoutSerializer(new DockingManager());
            tempSerializer.LayoutSerializationCallback += (sender, args) => fileIDs.Add(args.Model.ContentId);
            if (File.Exists(_layoutFile))
            {
                tempSerializer.Deserialize(_layoutFile);
            }
            bool everthingFound = _widgetManager.Widgets.Select(uiWidget => fileIDs.Any(fileID => fileID.ToLower().Equals(uiWidget.ContentGuid.ToLower()))).All(found => found);
            if (everthingFound)
            {
                var serializer = new XmlLayoutSerializer(dockingManager);
                if (File.Exists(_layoutFile))
                {
                    serializer.Deserialize(_layoutFile);
                }

            }
        }

        void IOperationViewer.OnNewOperation(Operation operation)
        {
            foreach (IUIWidget uiWidget in _widgetManager.Widgets)
            {
                try
                {
                    uiWidget.OnOperationChange(operation);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, uiWidget, Properties.Resources.OperationChangeFailed, uiWidget.Title);
                    Logger.Instance.LogException(this, ex);
                }
            }
        }

        void IOperationViewer.OnOperationChanged(Operation operation)
        {
            foreach (IUIWidget uiWidget in _widgetManager.Widgets)
            {
                try
                {
                    uiWidget.OnOperationChange(operation);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, uiWidget, Properties.Resources.OperationChangeFailed, uiWidget.Title);
                    Logger.Instance.LogException(this, ex);
                }
            }
        }

        FrameworkElement IOperationViewer.Visual
        {
            get { return this; }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            var serializer = new XmlLayoutSerializer(dockingManager);
            XmlTextWriter writer = new XmlTextWriter(_layoutFile, Encoding.UTF8);
            serializer.Serialize(writer);
            writer.Flush();
            writer.Close();

        }
    }
}