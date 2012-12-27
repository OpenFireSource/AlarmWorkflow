
namespace AlarmWorkflow.Job.GrowlJob
{
    /// <summary>
    /// Defines a means for a type that can send Growl notifications to a specific target.
    /// See documentation for further information.
    /// </summary>
    /// <remarks>Implement and export this interface to extend the Growl Job to send to custom Growl targets (websites, servers etc.).</remarks>
    public interface IGrowlSender
    {
        /// <summary>
        /// Sends a notification to the growl target.
        /// </summary>
        /// <param name="growl">The parent IGrowlJob.</param>
        /// <param name="headline">The headline/summary of the growl notification.</param>
        /// <param name="text">The text to send.</param>
        void SendNotification(IGrowlJob growl, string headline, string text);
    }
}
