using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Extensibility
{
    /// <summary>
    /// Defines a means for a type that is able to provide route images from a predefined start (site) to destination (operation).
    /// </summary>
    public interface IRouteImageProvider
    {
        byte[] GetRouteImage(PropertyLocation start, PropertyLocation destination, IDictionary<string, string> options);
    }
}
