using System.Windows;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.Configuration.AddressBookEditor.Extensibility;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor.CustomDataEditors
{
    /// <summary>
    /// Interaction logic for MailCustomDataEditor.xaml
    /// </summary>
    [Export("MailCustomDataEditor", typeof(ICustomDataEditor))]
    [Information(DisplayName = "INF_MailCustomDataEditor", Tag = "Mail")]
    public partial class MailCustomDataEditor : UserControl, ICustomDataEditor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MailCustomDataEditor"/> class.
        /// </summary>
        public MailCustomDataEditor()
        {
            InitializeComponent();
        }

        #endregion

        #region ITypeEditor Members

        object ITypeEditor.Value
        {
            get
            {
                // TODO: Verify format first! The method below will return "null" if parsing failed.
                MailAddressEntryObject meo = MailAddressEntryObject.FromAddress(txtText.Text);

                return meo;
            }
            set
            {
                MailAddressEntryObject meo = (MailAddressEntryObject)value;

                txtText.Text = meo.Address.Address;
            }
        }

        /// <summary>
        /// Gets the visual element that is editing the value.
        /// </summary>
        public UIElement Visual
        {
            get { return this; }
        }

        void ITypeEditor.Initialize(string editorParameter)
        {

        }

        #endregion
    }
}
