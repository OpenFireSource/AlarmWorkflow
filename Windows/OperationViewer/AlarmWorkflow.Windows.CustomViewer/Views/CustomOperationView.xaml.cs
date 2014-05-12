// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

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
    /// Interaction logic for CustomOperationViewer.xaml
    /// </summary>
    [Export("CustomOperationViewer", typeof(IOperationViewer))]
    [Information(DisplayName = "ExportCowDisplayName", Description = "ExportCowDescription")]
    public partial class CustomOperationView : IOperationViewer
    {
        #region Constants

        private const string LayoutFileName = "CustomOperationViewer.layout";
        private readonly string _layoutFile = Path.Combine(Utilities.GetLocalAppDataFolderPath(), LayoutFileName);

        #endregion

        #region Fields

        private readonly WidgetManager _widgetManager;
        private Operation _operation;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomOperationView"/> class.
        /// </summary>
        public CustomOperationView()
        {
            InitializeComponent();

            _widgetManager = new WidgetManager();
            InitializeWidgetManager();
        }

        #endregion

        #region Methods

        private void InitializeWidgetManager()
        {
            foreach (ILayoutPanelElement layoutPanelElement in _widgetManager.GetInitializedViews())
            {
                rootLayout.RootPanel.Children.Add(layoutPanelElement);
            }

            if (File.Exists(_layoutFile))
            {
                IEnumerable<string> ids = GetIdentifiersInFile();

                bool everthingFound = _widgetManager.Widgets
                    .Select(widget => ids.Any(id => string.Equals(id, widget.ContentGuid, StringComparison.OrdinalIgnoreCase)))
                    .All(found => found);

                if (everthingFound)
                {
                    XmlLayoutSerializer serializer = new XmlLayoutSerializer(dockingManager);
                    serializer.Deserialize(_layoutFile);
                }
            }
        }

        private IEnumerable<string> GetIdentifiersInFile()
        {
            IList<string> ids = new List<string>();

            XmlLayoutSerializer tempSerializer = new XmlLayoutSerializer(new DockingManager());
            tempSerializer.LayoutSerializationCallback += (sender, args) =>
            {
                ids.Add(args.Model.ContentId);
            };

            tempSerializer.Deserialize(_layoutFile);

            return ids;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            XmlLayoutSerializer serializer = new XmlLayoutSerializer(dockingManager);

            using (XmlTextWriter writer = new XmlTextWriter(_layoutFile, Encoding.UTF8))
            {
                serializer.Serialize(writer);
            }
        }

        #endregion

        #region IOperationViewer Members

        void IOperationViewer.OnNewOperation(Operation operation)
        {
        }

        void IOperationViewer.OnOperationChanged(Operation operation)
        {
            if (Equals(_operation, operation))
            {
                return;
            }

            _operation = operation;
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

        #endregion
    }
}