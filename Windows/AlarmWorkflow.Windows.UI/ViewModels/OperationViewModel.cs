using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.UI.ViewModels
{
    class OperationViewModel : ViewModelBase
    {
        #region Fields

        private Lazy<ImageSource> _routeImage;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the operation that this VM is based on.
        /// </summary>
        public Operation Operation { get; set; }
        /// <summary>
        /// Gets the route image that is displayed.
        /// </summary>
        public ImageSource RouteImage
        {
            get { return _routeImage.Value; }
        }

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

            // This property is to be used with the RouteControl.
            _routeImage = new Lazy<ImageSource>(() =>
            {
                // Lazy-load route image?
                if (operation.RouteImage == null)
                {
                    return Helper.GetNoRouteImage();
                }

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = new MemoryStream(operation.RouteImage);
                image.EndInit();
                return image;
            });
        }

        #endregion

    }
}
