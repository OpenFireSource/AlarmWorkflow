using System;

namespace AlarmWorkflow.Job.GrowlJob
{
    /// <summary>
    /// Exposes the public interface of the GrowlJob to extensions.
    /// </summary>
    public interface IGrowlJob
    {
        /// <summary>
        /// Gets the "application name" to be used in growling.
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        /// Gets a string-array that contains the API-keys configured for the Growl Job.
        /// Depending on the extension, this may not be required.
        /// </summary>
        /// <returns>A string-array that contains the API-keys configured for the Growl Job.</returns>
        string[] GetApiKeys();
    }
}
