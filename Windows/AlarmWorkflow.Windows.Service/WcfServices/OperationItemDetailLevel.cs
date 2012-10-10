using System;

namespace AlarmWorkflow.Windows.Service.WcfServices
{
    /// <summary>
    /// Specifies the detail level, which is the level of information that is transferred over the web service.
    /// </summary>
    public enum OperationItemDetailLevel
    {
        /// <summary>
        /// Transfers only the minimum, baseline information. An operation will contain all information except for the Custom Data and the Route Image.
        /// Recommended for mobile devices.
        /// </summary>
        Minimum = 0,
        /// <summary>
        /// Transfers the full detail, which includes everything in the operation including Custom Data and the Route Image.
        /// </summary>
        Full = 1,
    }
}
