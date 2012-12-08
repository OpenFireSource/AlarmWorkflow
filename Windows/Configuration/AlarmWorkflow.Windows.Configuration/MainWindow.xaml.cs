using System.Windows;
using AlarmWorkflow.Windows.Configuration.ViewModels;

namespace AlarmWorkflow.Windows.Configuration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private MainViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            this.DataContext = _viewModel;
        }

        #endregion
    }
}
