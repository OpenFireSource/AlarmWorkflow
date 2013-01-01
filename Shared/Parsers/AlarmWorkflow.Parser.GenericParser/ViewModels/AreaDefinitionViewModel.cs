using System.Windows.Input;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Parser.GenericParser.ViewModels
{
    /// <summary>
    /// Defines the ViewModel that represents one area within a section.
    /// </summary>
    class AreaDefinitionViewModel : ViewModelBase
    {
        #region Fields

        private SectionDefinitionViewModel _parent;
        private string _name;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the name of the area.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        #endregion

        #region Commands

        #region Command "RemoveAreaCommand"

        /// <summary>
        /// The RemoveAreaCommand command.
        /// </summary>
        public ICommand RemoveAreaCommand { get; private set; }

        private void RemoveAreaCommand_Execute(object parameter)
        {
            _parent.Areas.Remove(this);
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaDefinitionViewModel"/> class.
        /// </summary>
        private AreaDefinitionViewModel()
        {
            Name = Properties.Resources.AreaBlankName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaDefinitionViewModel"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public AreaDefinitionViewModel(SectionDefinitionViewModel parent)
            : this()
        {
            Assertions.AssertNotNull(parent, "parent");
            _parent = parent;
        }

        #endregion
    }
}
