using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Specialized.Printing;
using AlarmWorkflow.Windows.ConfigurationContracts;

// TODO: This editor currently only supports one queue. In future, it will return a string[] for multiple queues.

namespace AlarmWorkflow.Windows.Configuration.TypeEditors.Specialized.Printing
{
    /// <summary>
    /// Interaction logic for PrintingQueueNamesTypeEditor.xaml
    /// </summary>
    [Export("PrintingQueueNamesTypeEditor", typeof(ITypeEditor))]
    public partial class PrintingQueueNamesTypeEditor : UserControl, ITypeEditor
    {
        #region Properties

        /// <summary>
        /// Gets the array of available printing queue names.
        /// </summary>
        public IEnumerable<string> Names { get; private set; }
        /// <summary>
        /// Gets/sets the list of all selected printing queue names.
        /// </summary>
        //public List<string> SelectedNames { get; set; }
        public string SelectedName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintingQueueNamesTypeEditor"/> class.
        /// </summary>
        public PrintingQueueNamesTypeEditor()
        {
            InitializeComponent();

            Names = PrintingQueueManager.GetInstance().Entries.Select(pq => pq.Name).OrderBy(p => p);

            this.DataContext = this;
        }

        #endregion

        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value
        {
            get { return SelectedName; }
            set { SelectedName = (string)value; }
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
