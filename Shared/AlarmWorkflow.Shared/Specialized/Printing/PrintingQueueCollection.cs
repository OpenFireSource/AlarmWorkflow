using System.Collections.ObjectModel;

namespace AlarmWorkflow.Shared.Specialized.Printing
{
    /// <summary>
    /// Represents a strongly-typed collection that manages <see cref="PrintingQueue"/>-items.
    /// </summary>
    public sealed class PrintingQueueCollection : Collection<PrintingQueue>
    {
        // TODO: Intentionally left blank... YET! Todo: checks that forbid having more than one queue with the same name (sanity-check).
    }
}
