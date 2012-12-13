using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Settings;
using AvalonDock.Layout;

namespace AlarmWorkflow.Windows.CustomViewer.Extensibility
{
    internal class WidgetManager
    {
        private List<ILayoutPanelElement> _PanelElements = new List<ILayoutPanelElement>();
        private List<IUIWidget> _Widgets = new List<IUIWidget>();

        public List<IUIWidget> Widgets
        {
            get { return _Widgets; }
            set { _Widgets = value; }
        }

        public List<ILayoutPanelElement> PanelElements
        {
            get { return _PanelElements; }
            set { _PanelElements = value; }
        }

        internal List<ILayoutPanelElement> InitializeViews()
        {
            ReadOnlyCollection<string> enabledWidgets = new ReadOnlyCollection<string>(SettingsManager.Instance.GetSetting("CustomOperationViewer", "WidgetConfiguration").GetValue<ExportConfiguration>().GetEnabledExports());
            foreach (ExportedType export in ExportedTypeLibrary.GetExports(typeof(IUIWidget)).Where(j => enabledWidgets.Contains(j.Attribute.Alias)))
            {
                var iuiWidget = export.CreateInstance<IUIWidget>();

                string jobName = iuiWidget.GetType().Name;
                Logger.Instance.LogFormat(LogType.Info, this, "Initializing ViewPlugin type '{0}'...", jobName);

                try
                {
                    if (!iuiWidget.Initialize())
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this, "ViewPlugin type '{0}' initialization failed. The ViewPlugin will not be executed.", jobName);
                        continue;
                    }
                    var pane =
                        new LayoutAnchorablePane(new LayoutAnchorable
                                                     {
                                                         Content = iuiWidget.UIElement,
                                                         ContentId = iuiWidget.ContentGuid,
                                                         Title = iuiWidget.Title,
                                                         CanClose = false,
                                                         CanHide = false
                                                     });

                    _PanelElements.Add(pane);
                    _Widgets.Add(iuiWidget);
                    Logger.Instance.LogFormat(LogType.Info, this, "ViewPlugin type '{0}' initialization successful.", jobName);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, "An error occurred while initializing ViewPlugin type '{0}'. The error message was: {1}", jobName, ex.Message);
                }
            }
            return _PanelElements;
        }
    }
}