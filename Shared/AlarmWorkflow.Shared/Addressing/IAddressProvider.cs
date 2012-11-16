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
        /// Parses the given XElement into a custom object.
        /// </summary>
        /// <param name="element">The XElement to parse.</param>
        /// <returns>The custom object.</returns>
        object ParseXElement(XElement element);
    }
}
