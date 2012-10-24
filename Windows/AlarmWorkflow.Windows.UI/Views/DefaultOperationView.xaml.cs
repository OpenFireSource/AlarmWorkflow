using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UI.Extensibility;
using AlarmWorkflow.Windows.UI.ViewModels;

namespace AlarmWorkflow.Windows.UI.Views
{
    /// <summary>
    /// Interaction logic for DefaultOperationView.xaml
    /// </summary>
    [Export("DefaultOperationView", typeof(IOperationViewer))]
    public partial class DefaultOperationView : UserControl, IOperationViewer
    {
        #region Fields

        private ViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultOperationView"/> class.
        /// </summary>
        public DefaultOperationView()
        {
            InitializeComponent();

            _viewModel = new ViewModel();
            this.DataContext = _viewModel;
        }

        #endregion

        #region IOperationViewer Members

        System.Windows.FrameworkElement IOperationViewer.Visual
        {
            get { return this; }
        }

        void IOperationViewer.OnNewOperation(Operation operation)
        {
        }

        void IOperationViewer.OnOperationChanged(Shared.Core.Operation operation)
        {
            _viewModel.Operation = operation;
        }

        #endregion

        #region Nested types

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
                    if (value == _operation)
                    {
                        return;
                    }

                    _operation = value;

                    OnPropertyChanged("Operation");
                    OnPropertyChanged("RoutePlanImage");
                }
            }

            /// <summary>
            /// Gets the image of the route plan (if available).
            /// </summary>
            public ImageSource RoutePlanImage
            {
                get { return GetRoutePlanImage(); }
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

            #region Methods

            private ImageSource GetRoutePlanImage()
            {
                if (_operation == null)
                {
                    return null;
                }
                if (_operation.RouteImage == null)
                {
                    // Return dummy image
                    return Helper.GetNoRouteImage();
                }

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = new MemoryStream(_operation.RouteImage);
                image.EndInit();
                return image;
            }

            #endregion

        }

        #endregion
    }
}
