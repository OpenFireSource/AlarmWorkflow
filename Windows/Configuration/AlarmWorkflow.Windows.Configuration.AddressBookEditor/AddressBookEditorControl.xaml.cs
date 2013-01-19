using System.Windows.Controls;
using AlarmWorkflow.Shared.Addressing;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.Configuration.AddressBookEditor.ViewModels;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor
{
    /// <summary>
    /// Interaction logic for AddressBookEditorControl.xaml
    /// </summary>
    [Export("AddressBookEditor", typeof(ITypeEditor))]
    public partial class AddressBookEditorControl : UserControl, ISectionView
        , ITypeEditor // < DEBUG
    {
        #region Fields

        private AddressBookViewModel _viewModel;

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

        #region ITypeEditor Members

        object ITypeEditor.Value
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

        #region ISectionView Members

        void ISectionView.Save()
        {

        }

        #endregion
    }
}
