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
using System.ServiceModel;
using System.Windows;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Diagnostics.Reports;
using AlarmWorkflow.Windows.UI.Extensibility;
using AlarmWorkflow.Windows.UI.Models;
using AlarmWorkflow.Windows.UIContracts;

namespace AlarmWorkflow.Windows.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constants

        private const string ComponentName = "WindowsUI";

        #endregion

        #region Fields

        private readonly object Lock = new object();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configuration for the UI.
        /// </summary>
        internal UIConfiguration Configuration { get; private set; }
        /// <summary>
        /// Gets the extension manager instance.
        /// </summary>
        internal ExtensionManager ExtensionManager { get; private set; }
        /// <summary>
        /// Gets whether the application is first started or if it is already running longer.
        /// </summary>
        internal bool StartRoutine { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            Logger.Instance.Initialize(ComponentName);
            ErrorReportManager.RegisterAppDomainUnhandledExceptionListener(ComponentName);

            LoadConfiguration();
            StartRoutine = true;
        }

        #endregion

        #region Methods

        internal static App GetApp()
        {
            return (App)App.Current;
        }

        private void LoadConfiguration()
        {
            try
            {
                AlarmWorkflow.Windows.UI.Properties.Settings.Default.Reload();

                Configuration = new UIConfiguration();
                ExtensionManager = new ExtensionManager();

                Logger.Instance.LogFormat(LogType.Info, this, AlarmWorkflow.Windows.UI.Properties.Resources.UIConfigurationLoaded);
            }
            catch (EndpointNotFoundException ex)
            {
                UIUtilities.ShowError(AlarmWorkflow.Windows.UI.Properties.Resources.UICannotStartWithoutConnectionError);

                Logger.Instance.LogException(this, ex);
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, AlarmWorkflow.Windows.UI.Properties.Resources.UIConfigurationLoadError);
                Logger.Instance.LogException(this, ex);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Exit"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            AlarmWorkflow.Windows.UI.Properties.Settings.Default.Save();
        }

        #endregion

    }
}