using System;
using System.Windows;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Diagnostics.Reports;
using AlarmWorkflow.Windows.UI.Extensibility;
using AlarmWorkflow.Windows.UI.Models;
using AlarmWorkflow.Windows.UIContracts.Security;

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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            Logger.Instance.Initialize(ComponentName);
            ErrorReportManager.RegisterAppDomainUnhandledExceptionListener(ComponentName);

            AlarmWorkflow.Shared.Settings.SettingsManager.Instance.Initialize();
            LoadConfiguration();
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
                Configuration = UIConfiguration.Load();
                Logger.Instance.LogFormat(LogType.Info, this, AlarmWorkflow.Windows.UI.Properties.Resources.UIConfigurationLoaded);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, AlarmWorkflow.Windows.UI.Properties.Resources.UIConfigurationLoadError);
                Logger.Instance.LogException(this, ex);

                // Use default configuration
                Configuration = new UIConfiguration();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeServices();
            ExtensionManager = new ExtensionManager();

            AlarmWorkflow.Windows.UI.Properties.Settings.Default.Reload();
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

        private void InitializeServices()
        {
            ServiceProvider.Instance.AddService(typeof(ICredentialConfirmationDialogService), new Security.CredentialConfirmationDialogService());
        }

        #endregion

    }
}
