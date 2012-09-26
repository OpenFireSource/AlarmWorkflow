
namespace AlarmWorkflow.Shared.Logging
{
    /// <summary>
    /// The NoLoger class, log all events to nothing.
    /// </summary>
    public class NoLogger : ILogger
    {
        #region ILogger Members

        bool ILogger.IsEnabled { get; set; }

        bool ILogger.Initialize()
        {
            return false;
        }

        void ILogger.WriteInformation(string info)
        {
        }

        void ILogger.WriteWarning(string warning)
        {
        }

        void ILogger.WriteError(string errorMessage)
        {
        }

        #endregion

        #region IDisposable Members

        void System.IDisposable.Dispose()
        {

        }

        #endregion
    }
}
