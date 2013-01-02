using System.Windows;
using System.Windows.Controls;
using AlarmWorkflow.Parser.GenericParser.ViewModels;

namespace AlarmWorkflow.Parser.GenericParser.Views
{
    /// <summary>
    /// Interaction logic for EditControl.xaml
    /// </summary>
    public partial class EditControl : UserControl
    {
        #region Fields

        private ParserDefinitionViewModel _viewModel;

        #endregion

        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EditControl"/> class.
        /// </summary>
        public EditControl()
        {
            InitializeComponent();

            this.Loaded += EditControl_Loaded;
        }

        #endregion

        #region Event handlers

        private void EditControl_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO: Use ServiceProvider etc. instead of Load-event.
            this.Loaded -= EditControl_Loaded;

            _viewModel = ((MainWindowViewModel)this.DataContext).ParserDefinition;
            this.DataContext = _viewModel;
        }

        #endregion

    }
}
