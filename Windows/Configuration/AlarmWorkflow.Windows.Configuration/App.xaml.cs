using System.Windows;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Diagnostics.Reports;

namespace AlarmWorkflow.Windows.Configuration
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constants

        private const string ComponentName = "Configuration";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            Logger.Instance.Initialize(ComponentName);
            ErrorReportManager.RegisterAppDomainUnhandledExceptionListener(ComponentName);
        }

        #endregion

    }
}
