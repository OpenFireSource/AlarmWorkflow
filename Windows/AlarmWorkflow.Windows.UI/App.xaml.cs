using System;
using System.Windows;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
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
        /// Calling this default constructor will cause an exception, because it is not allowed to start the UI manually
        /// (it must be started through the <see cref="UINotifyable"/> type)!
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">More than one instance of the <see cref="T:System.Windows.Application"/> class is created per <see cref="T:System.AppDomain"/>.</exception>
        public App()
        {
            // Set up the logger for this instance
            Logger.Instance.Initialize("WindowsUI");

            AlarmWorkflow.Shared.Settings.SettingsManager.Instance.Initialize();
            LoadConfiguration();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the app.
        /// </summary>
        /// <returns></returns>
        internal static App GetApp()
        {
            return (App)App.Current;
        }

        private void LoadConfiguration()
        {
            try
            {
                Configuration = UIConfiguration.Load();
                Logger.Instance.LogFormat(LogType.Info, this, "The UI-configuration was successfully loaded.");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, "The UI-configuration could not be loaded! Using default configuration.");
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
        }

        /// <summary>
        /// Initializes the services and registers them at the ServiceProvider.
        /// </summary>
        private void InitializeServices()
        {
            // Credentials-confirmation dialog service
            ServiceProvider.Instance.AddService(typeof(ICredentialConfirmationDialogService), new Security.CredentialConfirmationDialogService());
        }

        #endregion

    }
}
