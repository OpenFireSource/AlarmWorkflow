
namespace AlarmWorkflow.Shared.Extensibility
{
    /// <summary>
    /// Defines the mechanisms for a type that hosts extensions. All extensions need to register at this host.
    /// </summary>
    public interface IExtensionHost
    {
        /// <summary>
        /// Registers the given job at the extension host.
        /// </summary>
        /// <param name="job">The job to register.</param>
        void RegisterJob(IJob job);
        /// <summary>
        /// Registers the given parser at the extension host.
        /// </summary>
        /// <param name="parser">The parser to register.</param>
        void RegisterParser(IParser parser);
    }
}
