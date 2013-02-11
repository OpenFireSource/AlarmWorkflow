using System.ComponentModel;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for FDResourceConfigurationTypeEditor.xaml
    /// </summary>
    [Export("FDResourceConfigurationTypeEditor", typeof(ITypeEditor))]
    public partial class FDResourceConfigurationTypeEditor : UserControl, ITypeEditor, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Gets the FDResourceConfiguration-instance for editing.
        /// </summary>
        public FDResourceConfiguration Configuration { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FDResourceConfigurationTypeEditor"/> class.
        /// </summary>
        public FDResourceConfigurationTypeEditor()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        #endregion

        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value
        {
            get { return Configuration; }
            set
            {
                this.Configuration = FDResourceConfiguration.Parse((string)value);
                OnPropertyChanged("Configuration");
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

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when the property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var copy = PropertyChanged;
            if (copy != null)
            {
                copy(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
