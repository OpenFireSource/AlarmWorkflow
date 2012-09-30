
namespace AlarmWorkflow.Shared.Diagnostics
{
    /// <summary>
    /// The type of the logging message.
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Avoid using.
        /// </summary>
        None = 0,
        /// <summary>
        /// Log something on DEBUG Level (only when debug mode is true).
        /// Also used on Exceptions, since they can't be logged on console.
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Log something on TRACE level.
        /// Usually done when something is extremely unimportant yet interesting for record (i.e. error reports).
        /// </summary>
        Trace = 2,
        /// <summary>
        /// Log something on INFO Level.
        /// </summary>
        Info = 3,
        /// <summary>
        /// Log something on WARNING Level.
        /// </summary>
        Warning = 4,
        /// <summary>
        /// Log something on ERROR Level.
        /// </summary>
        Error = 5,
        /// <summary>
        /// Log something on EXCEPTION Level (only on Win32).
        /// These are not supposed to be used manually!
        /// </summary>
        Exception = 10,
        /// <summary>
        /// Message emitted by the console.
        /// </summary>
        Console = 11,
    }
}
