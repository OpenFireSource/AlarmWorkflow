using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.Configuration.AddressBookEditor.ViewModels;
using AlarmWorkflow.Windows.ConfigurationContracts;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Shared.Addressing;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor
{
    /// <summary>
    /// Interaction logic for AddressBookEditorControl.xaml
    /// </summary>
    [Export("AddressBookEditor", typeof(ITypeEditor))]
    public partial class AddressBookEditorControl : UserControl, ITypeEditor
    {
        #region Fields

        private MainViewModel _viewModel;

        #endregion

        #region Constructors

        public AddressBookEditorControl()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            this.DataContext = _viewModel;
        }

        #endregion

        #region ITypeEditor Members

        object ITypeEditor.Value
        {
            get
            {
                object value = null;
                StringSettingConvertibleTools.ConvertBack(_viewModel.AddressBookVM.AddressBook, out value);
                return value;
            }
            set
            {
                AddressBook ab = StringSettingConvertibleTools.ConvertFromSetting<AddressBook>(value);
                _viewModel.AddressBookVM.AddressBook = ab;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Windows.UIElement Visual
        {
            get { return this; }
        }

        void ITypeEditor.Initialize(string editorParameter)
        {

        }

        #endregion
    }
}
