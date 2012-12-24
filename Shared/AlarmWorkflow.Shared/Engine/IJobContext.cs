using System.Collections.Generic;

namespace AlarmWorkflow.Shared.Engine
{
    /// <summary>
    /// Defines an interface for a context, which is created by the engine and handed over to plugins.
    /// </summary>
    public interface IJobContext
    {
        /// <summary>
        /// Gets the name of the alarm source that this context runs in.
        /// </summary>
        string AlarmSourceName { get; }
        /// <summary>
        /// Gets an alarm-source specific array of parameters that are associated with this context and alarm source.
        /// This property is filled by the alarm source.
        /// </summary>
        IDictionary<string, object> Parameters { get; }
    }
}
