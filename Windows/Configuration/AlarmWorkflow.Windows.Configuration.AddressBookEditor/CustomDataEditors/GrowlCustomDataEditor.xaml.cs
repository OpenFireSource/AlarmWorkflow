using System.Windows.Controls;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.Configuration.AddressBookEditor.Extensibility;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor.CustomDataEditors
{
    /// <summary>
    /// Interaction logic for GrowlCustomDataEditor.xaml
    /// </summary>
    [Export("GrowlCustomDataEditor", typeof(ICustomDataEditor))]
    [Information(DisplayName = "INF_GrowlCustomDataEditor", Tag = "Growl")]
    public partial class GrowlCustomDataEditor : UserControl, ICustomDataEditor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GrowlCustomDataEditor"/> class.
        /// </summary>
        public GrowlCustomDataEditor()
        {
            InitializeComponent();

            foreach (string dcon in GrowlEntryObject.DefaultConsumers)
            {
                cboConsumer.Items.Add(dcon);
            }
            cboConsumer.SelectedIndex = 0;
        }

        #endregion

        #region ITypeEditor Members

        object ConfigurationContracts.ITypeEditor.Value
        {
            get
            {
                GrowlEntryObject geo = new GrowlEntryObject();
                geo.Consumer = cboConsumer.Text;
                geo.RecipientApiKey = txtRecipientApiKey.Text;
                return geo;
            }
            set
            {
                GrowlEntryObject geo = (GrowlEntryObject)value;
                cboConsumer.Text = geo.Consumer;
                txtRecipientApiKey.Text = geo.RecipientApiKey;
            }
        }

        /// <summary>
        /// Gets the visual element that is editing the value.
        /// </summary>
        public System.Windows.UIElement Visual
        {
            get { return this; }
        }

        void ConfigurationContracts.ITypeEditor.Initialize(string editorParameter)
        {

        }

        #endregion
    }
}
