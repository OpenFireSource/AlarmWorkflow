using System.Drawing;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Extensibility
{
    /// <summary>
    /// Defines a means for a type that is able to provide route images from a predefined start (site) to destination (operation).
    /// </summary>
    public interface IRoutePlanProvider
    {
        /// <summary>
        /// Creates a requested for the route image provider and returns the image contents of the route image.
        /// </summary>
        /// <param name="start">The start location.</param>
        /// <param name="destination">The destination location (from the Operation).</param>
        /// <returns>The image contents of the route image. -or- null, if no route image could be retrieved (no connection, invalid addresses etc.).</returns>
        Image GetRouteImage(PropertyLocation start, PropertyLocation destination);
    }
}
