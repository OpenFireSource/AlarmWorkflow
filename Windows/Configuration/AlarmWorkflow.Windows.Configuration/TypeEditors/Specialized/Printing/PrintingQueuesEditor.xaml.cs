using System.Windows.Controls;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Shared.Specialized.Printing;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors.Specialized.Printing
{
    /// <summary>
    /// Interaction logic for PrintingQueuesEditor.xaml
    /// </summary>
    public partial class PrintingQueuesEditor : UserControl
    {
        #region Fields

        private PrintingQueuesEditorViewModel _viewModel;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current value by creating an addressbook out of all current entries, and sets the value from the settings manager.
        /// </summary>
        internal object ValueWrapper
        {
            get
            {
                object value = null;
                StringSettingConvertibleTools.ConvertBack(_viewModel.EditWrapper, out value);
                return value;
            }
            set
            {
                PrintingQueuesConfiguration pqc = StringSettingConvertibleTools.ConvertFromSetting<PrintingQueuesConfiguration>(value);
                _viewModel.EditWrapper = pqc;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintingQueuesEditor"/> class.
        /// </summary>
        public PrintingQueuesEditor()
        {
            InitializeComponent();

            _viewModel = new PrintingQueuesEditorViewModel();
            this.DataContext = _viewModel;
        }

        #endregion
    }
}
