using System.Windows;
using AlarmWorkflow.Windows.Configuration.ViewModels;
using AlarmWorkflow.Windows.UIContracts;

namespace AlarmWorkflow.Windows.Configuration.Views
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        #region Fields

        private AboutWindowViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutWindow"/> class.
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();

            _viewModel = new AboutWindowViewModel();
            this.DataContext = _viewModel;
        }

        #endregion

        #region Methods

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            }
            catch (System.Exception ex)
            {
                UIUtilities.ShowWarning(Properties.Resources.HyperlinkRequestUriFailedMessage, e.Uri.AbsoluteUri, ex.Message);
            }
        }

        #endregion
    }
}
