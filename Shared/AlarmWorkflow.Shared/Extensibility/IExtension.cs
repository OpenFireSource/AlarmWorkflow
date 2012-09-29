using System;

namespace AlarmWorkflow.Shared.Extensibility
{
    /// <summary>
    /// Defines a means for any extension.
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        /// Initializes the extension using the provided extension host.
        /// </summary>
        /// <param name="host">The extension host to use.</param>
        void Initialize(IExtensionHost host);
        /// <summary>
        /// Shuts down the extension.
        /// </summary>
        void Shutdown();
    }
}
