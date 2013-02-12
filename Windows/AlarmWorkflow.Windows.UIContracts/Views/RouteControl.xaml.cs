using System.Windows;
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.UIContracts.Views
{
    /// <summary>
    /// Interaction logic for RouteControl.xaml
    /// </summary>
    public partial class RouteControl : UserControl
    {
        #region Properties

        /// <summary>
        /// Gets/sets the <see cref="T:Operation"/>-instance to get the route image from.
        /// </summary>
        public Operation Operation
        {
            get { return (Operation)GetValue(OperationProperty); }
            set { SetValue(OperationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Operation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OperationProperty = DependencyProperty.Register("Operation", typeof(Operation), typeof(RouteControl), new UIPropertyMetadata(null));

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteControl"/> class.
        /// </summary>
        public RouteControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
