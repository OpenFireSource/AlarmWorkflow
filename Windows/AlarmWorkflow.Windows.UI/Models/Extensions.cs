using System;
using System.Globalization;

namespace AlarmWorkflow.Windows.UI.Models
{
    /// <summary>
    /// Provides extensions for the UI.
    /// </summary>
    public static class ClientExtensions
    {
        /// <summary>
        /// Returns the Uri using the calling assembly and the given path.
        /// </summary>
        /// <param name="source">The instance that this method is called on. Used to infer assembly name for resource retrieval.</param>
        /// <param name="path">The path. This is the text that follows the "component/" text.</param>
        /// <returns></returns>
        public static Uri GetPackUri(this object source, string path)
        {
            string applicationName = source.GetType().Assembly.GetName().Name;

            return new Uri(string.Format(CultureInfo.InvariantCulture, "pack://application:,,,/{0};component/{1}", applicationName, path));
        }
    }
}
