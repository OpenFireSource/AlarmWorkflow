using System.Windows;
using AlarmWorkflow.Windows.ServiceMonitor.ViewModel;

namespace AlarmWorkflow.Windows.ServiceMonitor
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowModel(this);
            DataContext = _viewModel;
        }
    }
}