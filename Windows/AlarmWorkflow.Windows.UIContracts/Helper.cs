using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AlarmWorkflow.Windows.UIContracts
{
    /// <summary>
    /// Provides some utilities when working with the Windows UI.
    /// </summary>
    internal static class Helper
    {
        private const string NoRouteImagePath = "Images/NoRouteImage.png";

        /// <summary>
        /// Returns the content from the "No route image" image as an image.
        /// </summary>
        /// <returns></returns>
        public static ImageSource GetNoRouteImage()
        {
            return new BitmapImage(ClientExtensions.GetPackUri("AlarmWorkflow.Windows.UIContracts", NoRouteImagePath));
        }
    }
}
