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
        private List<ILayoutPanelElement> _panelElements = new List<ILayoutPanelElement>();
        private List<IUIWidget> _widgets = new List<IUIWidget>();

        public List<IUIWidget> Widgets
        {
            get { return _widgets; }
            set { _widgets = value; }
        }

        public List<ILayoutPanelElement> PanelElements
        {
            get { return _panelElements; }
            set { _panelElements = value; }
        }

        internal List<ILayoutPanelElement> InitializeViews()
        {
            ReadOnlyCollection<string> enabledWidgets = new ReadOnlyCollection<string>(SettingsManager.Instance.GetSetting("CustomOperationViewer", "WidgetConfiguration").GetValue<ExportConfiguration>().GetEnabledExports());
            foreach (ExportedType export in ExportedTypeLibrary.GetExports(typeof(IUIWidget)).Where(j => enabledWidgets.Contains(j.Attribute.Alias)))
            {
                var iuiWidget = export.CreateInstance<IUIWidget>();

                string iuiWidgetName = iuiWidget.GetType().Name;
                Logger.Instance.LogFormat(LogType.Info, this, Properties.Resources.Init, iuiWidgetName);

                try
                {
                    if (!iuiWidget.Initialize())
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.InitFailed, iuiWidgetName);
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

                    _panelElements.Add(pane);
                    _widgets.Add(iuiWidget);
                    Logger.Instance.LogFormat(LogType.Info, this, Properties.Resources.InitSuccessful, iuiWidgetName);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.InitError, iuiWidgetName, ex.Message);
                }
            }
            return _panelElements;
        }
    }
}