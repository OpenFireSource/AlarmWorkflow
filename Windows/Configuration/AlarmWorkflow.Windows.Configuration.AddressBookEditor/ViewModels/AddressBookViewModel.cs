using System.Collections.ObjectModel;
using System.Windows.Input;
using AlarmWorkflow.Shared.Addressing;
using AlarmWorkflow.Windows.UIContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor.ViewModels
{
    /// <summary>
    /// Wrapper for an <see cref="AddressBookEditWrapper"/>-instance.
    /// </summary>
    class AddressBookViewModel : ViewModelBase
    {
        #region Fields

        private EntryViewModel _selectedEntry;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the <see cref="AddressBookEditWrapper"/>-instance to be edited.
        /// </summary>
        public AddressBook AddressBookEditWrapper
        {
            get { return CompileAddressBookFromViewModel(); }
            private set { SetAddressBookAndUpdateViewModel(value); }
        }


        private void SetAddressBookAndUpdateViewModel(AddressBook addressBook)
        {
            Entries = new ObservableCollection<EntryViewModel>();
            foreach (AddressBookEntry abe in addressBook.Entries)
            {
                EntryViewModel evm = new EntryViewModel();
                evm.Source = abe;

                foreach (EntryDataItem edi in abe.Data)
                {
                    EntryDataItemViewModel edivm = new EntryDataItemViewModel(evm);
                    edivm.IsEnabled = edi.IsEnabled;
                    edivm.Source = edi;
                    edivm.Editor = CustomDataEditors.CustomDataEditorCache.CreateTypeEditor(edi.Identifier);
                    if (edivm.Editor != null)
                    {
                        edivm.Editor.Value = edi.Data;
                    }

                    evm.DataItems.Add(edivm);
                }

                Entries.Add(evm);
            }

            if (Entries.Count > 0)
            {
                SelectedEntry = Entries[0];
            }
            else
            {
                // TODO: Add blank entry
            }
        }

        private AddressBook CompileAddressBookFromViewModel()
        {
            AddressBook addressBook = new AddressBook();

            foreach (EntryViewModel evm in this.Entries)
            {
                AddressBookEntry abe = new AddressBookEntry();
                abe.FirstName = evm.FirstName;
                abe.LastName = evm.LastName;

                foreach (EntryDataItemViewModel edivm in evm.DataItems)
                {
                    EntryDataItem edi = new EntryDataItem();
                    // Decide which value to use
                    if (edivm.Editor != null)
                    {
                        // TODO: Add checkbox to GroupBox that determines the IsEnabled-state
                        edi.IsEnabled = edivm.IsEnabled;
                        edi.Identifier = CustomDataEditors.CustomDataEditorCache.GetTypeEditorIdentifier(edivm.Editor.GetType());
                        edi.Data = edivm.Editor.Value;
                    }
                    else
                    {
                        if (edivm.Source != null)
                        {
                            edi = edivm.Source;
                        }
                        else
                        {
                            // TODO: What happens if there is no editor, AND no value?
                        }
                    }

                    abe.Data.Add(edi);
                }

                addressBook.Entries.Add(abe);
            }


            return addressBook;
        }



        /// <summary>
        /// Gets the collection of entries to display in the UI.
        /// </summary>
        public ObservableCollection<EntryViewModel> Entries { get; private set; }
        /// <summary>
        /// Gets/sets the currently selected entry.
        /// </summary>
        public EntryViewModel SelectedEntry
        {
            get { return _selectedEntry; }
            set
            {
                _selectedEntry = value;
                OnPropertyChanged("SelectedEntry");
            }
        }

        #endregion

        #region Commands

        #region Command "AddEntryCommand"

        /// <summary>
        /// The AddEntryCommand command.
        /// </summary>
        public ICommand AddEntryCommand { get; private set; }

        private void AddEntryCommand_Execute(object parameter)
        {
            EntryViewModel evmNew = new EntryViewModel();
            evmNew.Source = new AddressBookEntry();

            this.Entries.Add(evmNew);
            this.SelectedEntry = evmNew;
        }

        #endregion

        #region Command "RemoveEntryCommand"

        /// <summary>
        /// The RemoveEntryCommand command.
        /// </summary>
        public ICommand RemoveEntryCommand { get; private set; }

        private bool RemoveEntryCommand_CanExecute(object parameter)
        {
            return SelectedEntry != null;
        }

        private void RemoveEntryCommand_Execute(object parameter)
        {
            if (!UIUtilities.ConfirmMessageBox(System.Windows.MessageBoxImage.Question, Properties.Resources.ConfirmDeleteEntry))
            {
                return;
            }

            Entries.Remove(this.SelectedEntry);
            this.SelectedEntry = null;
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressBookViewModel"/> class.
        /// </summary>
        /// <param name="addressBook"></param>
        public AddressBookViewModel(AddressBook addressBook)
        {
            AddressBookEditWrapper = addressBook;
        }

        #endregion
    }
}
