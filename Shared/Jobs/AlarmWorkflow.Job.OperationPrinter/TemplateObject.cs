using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Job.OperationPrinter
{
    /// <summary>
    /// Template-Object used when formatting the HTML-page.
    /// </summary>
    sealed class TemplateObject
    {
        /// <summary>
        /// Gets/sets the operation that is printed.
        /// </summary>
        public Operation Operation { get; set; }
        /// <summary>
        /// Gets/sets the full file path of the route image file.
        /// </summary>
        public string RouteImageFilePath { get; set; }
    }
}
