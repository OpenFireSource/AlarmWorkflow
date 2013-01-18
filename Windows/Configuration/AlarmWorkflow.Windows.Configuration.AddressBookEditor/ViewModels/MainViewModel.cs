using System;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets/sets the address book VM.
        /// </summary>
        public AddressBookViewModel AddressBookVM { get; set; }

        #endregion

        #region Constructors

        public MainViewModel()
        {
            AddressBookVM = new AddressBookViewModel();
        }

        #endregion
    }
}
