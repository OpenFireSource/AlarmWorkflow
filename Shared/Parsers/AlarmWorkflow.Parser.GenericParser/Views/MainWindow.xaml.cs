using System.Windows;
using AlarmWorkflow.Parser.GenericParser.ViewModels;

namespace AlarmWorkflow.Parser.GenericParser.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private MainWindowViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            this.DataContext = _viewModel;
        }

        #endregion
    }
}
