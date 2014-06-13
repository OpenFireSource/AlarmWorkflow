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
using System.Linq;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AvalonDock.Layout;

namespace AlarmWorkflow.Windows.CustomViewer.Extensibility
{
    class WidgetManager
    {
        #region Fields

        private IList<ILayoutPanelElement> _panelElements;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of all widgets.
        /// </summary>
        public IList<IUIWidget> Widgets { get; private set; }

        #endregion

        #region Constructors

        internal WidgetManager()
        {
            _panelElements = new List<ILayoutPanelElement>();

            Widgets = new List<IUIWidget>();
        }

        #endregion

        #region Methods

        internal IEnumerable<ILayoutPanelElement> GetInitializedViews()
        {
            IEnumerable<string> enabledWidgets = GetEnabledWidgets();

            foreach (ExportedType export in ExportedTypeLibrary.GetExports(typeof(IUIWidget)).Where(j => enabledWidgets.Contains(j.Attribute.Alias)))
            {
                IUIWidget widget = export.CreateInstance<IUIWidget>();

                string widgetName = widget.GetType().Name;
                Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.BeginInitialization, widgetName);

                try
                {
                    if (!widget.Initialize())
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.InitializationFailure, widgetName);
                        continue;
                    }

                    LayoutAnchorablePane pane = CreatePaneFromWidget(widget);
                    _panelElements.Add(pane);

                    Widgets.Add(widget);

                    Logger.Instance.LogFormat(LogType.Trace, this, Properties.Resources.InitializationSuccess, widgetName);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.InitializationError, widgetName);
                    Logger.Instance.LogException(this, ex);
                }
            }

            return _panelElements;
        }

        private static LayoutAnchorablePane CreatePaneFromWidget(IUIWidget widget)
        {
            LayoutAnchorable anchorable = new LayoutAnchorable();
            anchorable.Content = widget.UIElement;
            anchorable.ContentId = widget.ContentGuid;
            anchorable.Title = widget.Title;
            anchorable.CanClose = false;
            anchorable.CanHide = false;

            LayoutAnchorablePane pane = new LayoutAnchorablePane(anchorable);
            return pane;
        }

        private static IEnumerable<string> GetEnabledWidgets()
        {
            using (var service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(new SettingsServiceCallback()))
            {
                ExportConfiguration exports = service.Instance.GetSetting(SettingKeys.WidgetConfigurationKey).GetValue<ExportConfiguration>();
                return exports.GetEnabledExports();
            }
        }

        #endregion
    }
}