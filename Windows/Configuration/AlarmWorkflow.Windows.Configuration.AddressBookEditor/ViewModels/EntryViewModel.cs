using System.Collections.ObjectModel;
using System.Windows.Input;
using AlarmWorkflow.Shared.Addressing;
using AlarmWorkflow.Windows.Configuration.AddressBookEditor.Extensibility;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor.ViewModels
{
    /// <summary>
    /// Wrapper for an <see cref="AddressBookEntry"/>-item.
    /// </summary>
    class EntryViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets/sets the source entry that is the base of this VM.
        /// </summary>
        public AddressBookEntry Source { get; set; }
        /// <summary>
        /// Gets/sets the wrapper for "Source.FirstName" with change notification.
        /// </summary>
        public string FirstName
        {
            get { return Source.FirstName; }
            set
            {
                Source.FirstName = value;
                OnPropertyChanged("FirstName");
            }
        }
        /// <summary>
        /// Gets/sets the wrapper for "Source.FirstName" with change notification.
        /// </summary>
        public string LastName
        {
            get { return Source.LastName; }
            set
            {
                Source.LastName = value;
                OnPropertyChanged("LastName");
            }
        }
        /// <summary>
        /// Gets/sets the wrapper for "Source.Data" with change notification.
        /// </summary>
        public ObservableCollection<EntryDataItemViewModel> DataItems { get; private set; }
        /// <summary>
        /// Gets/sets the currently selected entry.
        /// </summary>
        public EntryDataItemViewModel SelectedEntry { get; set; }

        #endregion

        #region Commands

        #region Command "AddDataItemCommand"

        /// <summary>
        /// The AddDataItemCommand command.
        /// </summary>
        public ICommand AddDataItemCommand { get; private set; }

        private void AddDataItemCommand_Execute(object parameter)
        {
            string displayName = (string)parameter;

            ICustomDataEditor editor = CustomDataEditors.CustomDataEditorCache.CreateTypeEditorFromDisplayName(displayName);

            EntryDataItemViewModel vm = new EntryDataItemViewModel(this);
            vm.Editor = editor;

            DataItems.Add(vm);
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryViewModel"/> class.
        /// </summary>
        internal EntryViewModel()
        {
            DataItems = new ObservableCollection<EntryDataItemViewModel>();
        }

        #endregion
    }
}
