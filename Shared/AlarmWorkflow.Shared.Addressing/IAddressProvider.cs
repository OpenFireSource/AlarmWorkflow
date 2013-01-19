using System.Xml.Linq;

namespace AlarmWorkflow.Shared.Addressing
{
    /// <summary>
    /// Defines a means for a type that provides and handles custom addresses (such as Pager address, phone address etc.).
    /// </summary>
    public interface IAddressProvider
    {
        /// <summary>
        /// Returns a string that identifies the type of the address this provider handles.
        /// </summary>
        string AddressType { get; }

        /// <summary>
        /// Converts the given <see cref="XElement"/> into a specific .Net object.
        /// </summary>
        /// <param name="element">The XElement to convert.</param>
        /// <returns>The specific .Net object.</returns>
        object Convert(XElement element);
        /// <summary>
        /// Converts the given specific .Net object from this instance into a serializable <see cref="XElement"/>.
        /// </summary>
        /// <param name="value">The specific .Net object to serialize.</param>
        /// <returns>The serializable <see cref="XElement"/> that contains the data from the specific object.</returns>
        XElement ConvertBack(object value);
    }
}
