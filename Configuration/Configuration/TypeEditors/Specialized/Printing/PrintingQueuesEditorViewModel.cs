// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.ObjectModel;
using System.Windows.Input;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Specialized.Printing;
using AlarmWorkflow.Windows.UIContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors.Specialized.Printing
{
    class PrintingQueuesEditorViewModel : ViewModelBase
    {
        #region Fields

        private PrintingQueue _selectedEntry;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="PrintingQueuesConfiguration"/> to be edited.
        /// </summary>
        public PrintingQueuesConfiguration EditWrapper
        {
            get { return Compile(); }
            set { SetFrom(value); }
        }

        /// <summary>
        /// Gets the collection of entries.
        /// </summary>
        public ObservableCollection<PrintingQueue> Entries { get; private set; }
        /// <summary>
        /// Gets/sets the currently selected entry.
        /// </summary>
        public PrintingQueue SelectedEntry
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
            PrintingQueue evmNew = new PrintingQueue();

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
            if (!UIUtilities.ConfirmMessageBox(System.Windows.MessageBoxImage.Question, Properties.Resources.PrintingQueuesConfigurationWindowConfirmDeleteEntry))
            {
                return;
            }

            this.Entries.Remove(this.SelectedEntry);
            this.SelectedEntry = null;
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintingQueuesEditorViewModel"/> class.
        /// </summary>
        public PrintingQueuesEditorViewModel()
        {
            Entries = new ObservableCollection<PrintingQueue>();
        }

        #endregion

        #region Methods

        private PrintingQueuesConfiguration Compile()
        {
            PrintingQueuesConfiguration tmp = new PrintingQueuesConfiguration();
            tmp.Entries.AddRange(this.Entries);

            return tmp;
        }

        private void SetFrom(PrintingQueuesConfiguration value)
        {
            this.Entries.AddRange(value.Entries);
        }

        #endregion
    }
}