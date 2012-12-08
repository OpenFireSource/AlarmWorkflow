using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.IlsAnsbachOperationViewer.ViewModels
{
    /// <summary>
    /// Represents the ViewModel that is used to display vehicle-specific data.
    /// </summary>
    class ResourceViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets/sets the display name of the vehicle.
        /// </summary>
        public string VehicleName { get; set; }
        /// <summary>
        /// Gets the image that represents the vehicle.
        /// </summary>
        public ImageSource Image { get; private set; }
        /// <summary>
        /// Gets/sets the equipment that is requested.
        /// </summary>
        public string RequestedEquipment { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the image from the given path.
        /// </summary>
        /// <param name="imagePath"></param>
        public void SetImage(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                return;
            }

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = File.OpenRead(imagePath);
            image.EndInit();

            this.Image = image;
        }

        #endregion

    }
}
