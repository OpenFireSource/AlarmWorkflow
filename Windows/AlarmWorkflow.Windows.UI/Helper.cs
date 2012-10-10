using System.Windows.Media;
using System.Windows.Media.Imaging;
using AlarmWorkflow.Windows.UI.Models;

namespace AlarmWorkflow.Windows.UI
{
    /// <summary>
    /// Provides some utilities when working with the Windows UI.
    /// </summary>
    public static class Helper
    {
        private const string NoRouteImagePath = "Images/NoRouteImage.png";

        /// <summary>
        /// Returns the content from the "No route image" image as an image.
        /// </summary>
        /// <returns></returns>
        public static ImageSource GetNoRouteImage()
        {
            return new BitmapImage(ClientExtensions.GetPackUri("AlarmWorkflow.Windows.UI", NoRouteImagePath));
        }
    }
}
