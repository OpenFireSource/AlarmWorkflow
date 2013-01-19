using System.Windows.Input;
using AlarmWorkflow.Shared.Addressing;
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
