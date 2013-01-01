using System.Collections.ObjectModel;
using System.Windows.Input;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Parser.GenericParser.ViewModels
{
    /// <summary>
    /// Defines the ViewModel that represents the concrete Parser definition.
    /// </summary>
    class ParserDefinitionViewModel : ViewModelBase
    {
        #region Fields

        private string _parserName;
        private object _selectedNode;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the name of the parser (optional).
        /// </summary>
        public string ParserName
        {
            get { return _parserName; }
            set
            {
                _parserName = value;
                OnPropertyChanged("ParserName");
            }
        }
        /// <summary>
        /// Gets/sets the keyword separator string (usually a colon).
        /// </summary>
        public string KeywordSeparator { get; set; }
        /// <summary>
        /// Gets/sets a collection of sections that make up the parser definition.
        /// </summary>
        public ObservableCollection<SectionDefinitionViewModel> Sections { get; set; }

        /// <summary>
        /// Gets/sets the selected item. This is a quick-hack for WPF's TreeView which cannot do this out of the box.
        /// </summary>
        public object SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                _selectedNode = value;
                OnPropertyChanged("SelectedNode");
            }
        }

        #endregion

        #region Commands

        #region Command "AddSectionCommand"

        /// <summary>
        /// The AddSectionCommand command.
        /// </summary>
        public ICommand AddSectionCommand { get; private set; }

        private void AddSectionCommand_Execute(object parameter)
        {
            SectionDefinitionViewModel section = new SectionDefinitionViewModel(this);
            Sections.Add(section);
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserDefinitionViewModel"/> class.
        /// </summary>
        public ParserDefinitionViewModel()
        {
            ParserName = Properties.Resources.ParserDefinitionBlankName;
            KeywordSeparator = ":";
            Sections = new ObservableCollection<SectionDefinitionViewModel>();
        }

        #endregion
    }
}
