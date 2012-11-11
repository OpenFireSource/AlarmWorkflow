using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for ExportConfigurationTypeEditor.xaml
    /// </summary>
    [Export("ExportConfigurationTypeEditor", typeof(ITypeEditor))]
    public partial class ExportConfigurationTypeEditor : UserControl, ITypeEditor
    {
        #region Fields

        private object _value;
        private Type _exportedType;
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

                // Parse the export configuration
                _configuration = ExportConfiguration.Parse((string)_value);
                // Add/Remove exports from the ETL (if exported type is available)
                if (_exportedType != null)
                {
                    List<ExportedType> validExports = ExportedTypeLibrary.GetExports(_exportedType);

                    // Find out which exports are new to show in the list ...
                    var newExports = validExports.Select(e => e.Attribute.Alias).Except(_configuration.Exports.Select(e => e.Name));
                    // ... and which exports are no longer needed.
                    var obsoleteExports = _configuration.Exports.Select(e => e.Name).Except(validExports.Select(e => e.Attribute.Alias));

                    // Remove the ones we no longer need ...
                    _configuration.Exports.RemoveAll(e => obsoleteExports.Contains(e.Name));
                    // ... and add those we do need.
                    _configuration.Exports.AddRange(newExports.Select(e => new ExportConfiguration.ExportEntry() { Name = e }));
                }

                // Set reference to exports list for UI
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

        void ITypeEditor.Initialize(string editorParameter)
        {
            if (string.IsNullOrWhiteSpace(editorParameter))
            {
                return;
            }

            // Find out the type - if the type could not be found, go out.
            _exportedType = Type.GetType(editorParameter);
        }

        #endregion
    }
}
