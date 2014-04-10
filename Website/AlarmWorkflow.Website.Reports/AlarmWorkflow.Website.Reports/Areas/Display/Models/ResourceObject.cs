using AlarmWorkflow.BackendService.ManagementContracts.Emk;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Website.Reports.Areas.Display.Models
{
    /// <summary>
    /// A object for displaying resources in the alarmpage
    /// </summary>
    public class ResourceObject
    {
        /// <summary>
        /// A <see cref="EmkResource"/> if any is assigned to the resource
        /// </summary>
        public EmkResource Emk { get; set; }
        /// <summary>
        /// The <see cref="OperationResource"/> which should be displayed
        /// </summary>
        public OperationResource Resource { get; set; }

        /// <summary>
        /// The constructor for creating a new <see cref="ResourceObject"/>
        /// </summary>
        /// <param name="emk">The filtered entry. Can be null!</param>
        /// <param name="resource">The original entry. Can't be null!</param>
        public ResourceObject(EmkResource emk, OperationResource resource)
        {
            Assertions.AssertNotNull(resource, "resource");

            Emk = emk;
            Resource = resource;
        }
    }
}
