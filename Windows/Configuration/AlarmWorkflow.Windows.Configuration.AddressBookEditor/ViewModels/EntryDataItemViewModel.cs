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

using System.Windows.Input;
using AlarmWorkflow.BackendService.AddressingContracts;
using AlarmWorkflow.Windows.Configuration.AddressBookEditor.Extensibility;
using AlarmWorkflow.Windows.UIContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor.ViewModels
{
    /// <summary>
    /// Wrapper for an <see cref="EntryDataItem"/>-item.
    /// </summary>
    class EntryDataItemViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets the parent VM.
        /// </summary>
        public EntryViewModel Parent { get; private set; }

        /// <summary>
        /// Gets/sets the <see cref="EntryDataItem"/> that is the base of this VM.
        /// </summary>
        public EntryDataItem Source { get; set; }
        /// <summary>
        /// Gets/sets the editor that is used to edit this item.
        /// </summary>
        public ICustomDataEditor Editor { get; set; }
        /// <summary>
        /// Gets/sets whether or not this entry is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// Gets the display name of this data item.
        /// </summary>
        public string DisplayName
        {
            get { return Editor != null ?  AlarmWorkflow.Shared.Core.InformationAttribute.GetDisplayName(Editor.GetType()) : null; }
        }

        #endregion

        #region Commands

        #region Command "RemoveDataItemCommand"

        /// <summary>
        /// The RemoveDataItemCommand command.
        /// </summary>
        public ICommand RemoveDataItemCommand { get; private set; }

        private void RemoveDataItemCommand_Execute(object parameter)
        {
            if (!UIUtilities.ConfirmMessageBox(System.Windows.MessageBoxImage.Question, Properties.Resources.ConfirmDeleteEntry))
            {
                return;
            }
            this.Parent.DataItems.Remove(this);
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryDataItemViewModel"/> class.
        /// </summary>
        /// <param name="parent">The parent VM.</param>
        internal EntryDataItemViewModel(EntryViewModel parent)
        {
            Parent = parent;

            IsEnabled = true;
            Source = new EntryDataItem();
        }

        #endregion

    }
}