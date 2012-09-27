using System.Text;

namespace AlarmWorkflow.Windows.UI
{
    sealed class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private StringBuilder _logText;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            _logText = new StringBuilder(32 * 1024);
        }

        #endregion

        #region Methods

        #endregion
    }
}
