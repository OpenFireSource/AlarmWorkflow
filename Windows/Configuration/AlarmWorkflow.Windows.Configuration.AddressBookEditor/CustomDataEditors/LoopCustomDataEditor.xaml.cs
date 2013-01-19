using System.Windows.Controls;
using AlarmWorkflow.Shared.Addressing.EntryObjects;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.Configuration.AddressBookEditor.Extensibility;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor.CustomDataEditors
{
    /// <summary>
    /// Interaction logic for LoopCustomDataEditor.xaml
    /// </summary>
    [Export("LoopCustomDataEditor", typeof(ICustomDataEditor))]
    [Information(DisplayName = "INF_LoopCustomDataEditor", Tag = "Loop")]
    public partial class LoopCustomDataEditor : UserControl, ICustomDataEditor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoopCustomDataEditor"/> class.
        /// </summary>
        public LoopCustomDataEditor()
        {
            InitializeComponent();
        }

        #endregion

        #region ITypeEditor Members

        object ITypeEditor.Value
        {
            get
            {
                LoopEntryObject leo = new LoopEntryObject();
                leo.Loop = txtLoopCode.Text;

                return leo;
            }
            set
            {
                LoopEntryObject leo = (LoopEntryObject)value;

                txtLoopCode.Text = leo.Loop;
            }
        }

        /// <summary>
        /// Gets the visual element that is editing the value.
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
