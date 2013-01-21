using System.Windows.Media;
using System.Windows.Media.Imaging;
using AlarmWorkflow.Windows.UIContracts;
using System;
using System.Windows.Threading;

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

        /// <summary>
        /// Convenience wrapper for the "Dispatcher.Invoke()" method which does not support lambdas.
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="action"></param>
        internal static void Invoke(this Dispatcher dispatcher, Action action)
        {
            dispatcher.Invoke(action);
        }

        internal static double Limit(double min, double max, double value)
        {
            if (value < min)
            {
                return min;
            }
            if (value >max)
            {
                return max;
            }
            return value;
        }
    }
}
