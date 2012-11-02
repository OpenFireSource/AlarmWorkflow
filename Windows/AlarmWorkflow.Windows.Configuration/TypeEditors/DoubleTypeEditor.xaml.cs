using System.Globalization;
using System.Windows.Controls;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for DoubleTypeEditor.xaml
    /// </summary>
    public partial class DoubleTypeEditor : UserControl, ITypeEditor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleTypeEditor"/> class.
        /// </summary>
        public DoubleTypeEditor()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        #endregion

        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value { get; set; }
        //{
        //    get { return double.Parse(txtValue.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture); }
        //    set { txtValue.Text = ((double)value).ToString(CultureInfo.InvariantCulture); }
        //}

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
