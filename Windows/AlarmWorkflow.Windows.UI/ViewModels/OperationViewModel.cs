using System;
using System.Windows.Media;
using AlarmWorkflow.Shared.Core;

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

            // Lazy-load route image?
            if (operation.RouteImage != null)
            {
                //_routeImage = new Lazy<ImageSource>(() =>
                //{
                //    return MapsServiceHelper.GetRouteImage(pls,
                //        GetDestination(),
                //        App.GetApp().Configuration.RouteImageWidth,
                //        App.GetApp().Configuration.RouteImageHeight);
                //});
            }
        }

        #endregion

    }
}
