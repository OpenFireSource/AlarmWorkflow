using System;
using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AvalonDock.Layout;

namespace AlarmWorkflow.Windows.CustomViewer.Extensibility
{
    internal class ViewManager
    {
        internal List<ILayoutPanelElement> InitializeViews()
        {
            var views = new List<ILayoutPanelElement>();
            foreach (ExportedType export in ExportedTypeLibrary.GetExports(typeof(IUIWidget)))
            {
                var iuiWidget = export.CreateInstance<IUIWidget>();

                string jobName = iuiWidget.GetType().Name;
                Logger.Instance.LogFormat(LogType.Info, this, "Initializing ViewPlugin type '{0}'...", jobName);

                try
                {
                    if (!iuiWidget.Initialize())
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this,
                                                  "ViewPlugin type '{0}' initialization failed. The ViewPlugin will not be executed.",
                                                  jobName);
                        continue;
                    }
                    var pane = new LayoutAnchorablePane(new LayoutAnchorable { Content = iuiWidget.PanelElement });
                    views.Add(pane);
                    Logger.Instance.LogFormat(LogType.Info, this, "ViewPlugin type '{0}' initialization successful.",
                                              jobName);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this,
                                              "An error occurred while initializing ViewPlugin type '{0}'. The error message was: {1}",
                                              jobName, ex.Message);
                }
            }
            return views;
        }
    }
}