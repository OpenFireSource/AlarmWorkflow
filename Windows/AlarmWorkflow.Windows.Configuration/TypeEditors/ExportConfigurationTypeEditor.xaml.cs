using System.Collections.Generic;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for ExportConfigurationTypeEditor.xaml
    /// </summary>
    public partial class ExportConfigurationTypeEditor : UserControl, ITypeEditor
    {
        #region Fields

        private object _value;
        private ExportConfiguration _configuration;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of all exports.
        /// </summary>
        public List<ExportConfiguration.ExportEntry> Exports { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportConfigurationTypeEditor"/> class.
        /// </summary>
        public ExportConfigurationTypeEditor()
        {
            InitializeComponent();

            Exports = new List<ExportConfiguration.ExportEntry>();

            this.DataContext = this;
        }

        #endregion

        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value
        {
            get { return _configuration; }
            set
            {
                if (_value == value)
                {
                    return;
                }

                _value = value;

                _configuration = ExportConfiguration.Parse((string)_value);
                this.Exports = _configuration.Exports;
            }
        }

        /// <summary>
        /// Gets the visual element that is editing the value.
        /// </summary>
        public System.Windows.UIElement Visual
        {
            get { return this; }
        }

        #endregion
    }
}
