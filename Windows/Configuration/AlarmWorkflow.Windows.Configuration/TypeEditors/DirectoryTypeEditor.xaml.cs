using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for DirectoryTypeEditor.xaml
    /// </summary>
    [Export("DirectoryTypeEditor", typeof(ITypeEditor))]
    public partial class DirectoryTypeEditor : UserControl, ITypeEditor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryTypeEditor"/> class.
        /// </summary>
        public DirectoryTypeEditor()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void Browse_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.SelectedPath = (string)this.Value;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Value = fbd.SelectedPath;
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
