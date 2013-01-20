using System.Windows.Controls;
using AlarmWorkflow.Shared.Addressing;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.Configuration.AddressBookEditor.ViewModels;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor.Views
{
    /// <summary>
    /// Interaction logic for AddressBookEditorControl.xaml
    /// </summary>
    public partial class AddressBookEditorControl : UserControl
    {
        #region Fields

        private AddressBookViewModel _viewModel;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current value by creating an addressbook out of all current entries, and sets the value from the settings manager.
        /// </summary>
        internal object ValueWrapper
        {
            get
            {
                object value = null;
                StringSettingConvertibleTools.ConvertBack(_viewModel.AddressBookEditWrapper, out value);
                return value;
            }
            set
            {
                AddressBook ab = StringSettingConvertibleTools.ConvertFromSetting<AddressBook>(value);

                _viewModel = new AddressBookViewModel(ab);
                this.DataContext = _viewModel;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressBookEditorControl"/> class.
        /// </summary>
        public AddressBookEditorControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
