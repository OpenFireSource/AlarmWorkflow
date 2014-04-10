using System.Collections.Generic;

namespace AlarmWorkflow.Website.Reports.Areas.Display.Models
{
    /// <summary>
    /// A 'dummy' class which is required for the json result when polling the resources from a operation.
    /// </summary>
    public class ResourcesData
    {
        /// <summary>
        /// A list of <see cref="ResourceObject"/>s. This is required for the json result! (Accessing the data via the variable Resources)
        /// </summary>
        public List<ResourceObject> Resources { get; set; }
    }
}
