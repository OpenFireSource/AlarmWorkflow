using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for FileTypeEditor.xaml
    /// </summary>
    [Export("FileTypeEditor", typeof(ITypeEditor))]
    public partial class FileTypeEditor : UserControl, ITypeEditor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTypeEditor"/> class.
        /// </summary>
        public FileTypeEditor()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void Browse_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            if (this.Value != null)
            {
                sfd.FileName = (string)this.Value;
            }
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Value = sfd.FileName;
            }
        }

        #endregion

        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value
        {
            get { return txtValue.Text; }
            set { txtValue.Text = (string)value; }
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
