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

            _viewModel = new ParserDefinitionViewModel();
            this.DataContext = _viewModel;
        }

        #endregion

    }
}
