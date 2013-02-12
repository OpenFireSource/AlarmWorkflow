using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.UI.ViewModels
{
    class OperationViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets/sets the operation that this VM is based on.
        /// </summary>
        public Operation Operation { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationViewModel"/> class.
        /// </summary>
        public OperationViewModel()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationViewModel"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public OperationViewModel(Operation operation)
            : this()
        {
            this.Operation = operation;
        }

        #endregion

    }
}
