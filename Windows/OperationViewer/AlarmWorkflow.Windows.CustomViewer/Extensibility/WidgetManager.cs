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
using System.Collections.ObjectModel;
using System.Linq;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
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
            ReadOnlyCollection<string> enabledWidgets = GetEnabledWidgets();

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

        private static ReadOnlyCollection<string> GetEnabledWidgets()
        {
            using (var service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(new SettingsServiceCallback()))
            {
                ExportConfiguration exports = service.Instance.GetSetting(SettingKeys.WidgetConfigurationKey).GetValue<ExportConfiguration>();
                return new ReadOnlyCollection<string>(exports.GetEnabledExports());
            }
        }
    }
}