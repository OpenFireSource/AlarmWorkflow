using AlarmWorkflow.Windows.UI.ViewModels;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.IlsAnsbachOperationViewer
{
    class ViewModel : ViewModelBase
    {
        #region Fields

        private Operation _operation;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        public Operation Operation
        {
            get { return _operation; }
            set
            {
                _operation = value;

                // Set operation itself
                OnPropertyChanged("Operation");
                // Update all other properties
            }
        }

        

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {

        }

        #endregion

        #region Nested types

        class ResourceViewModel : ViewModelBase
        {

        }

        #endregion
    }
}
