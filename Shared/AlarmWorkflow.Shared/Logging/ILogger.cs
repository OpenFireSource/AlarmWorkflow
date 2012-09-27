using System;

namespace AlarmWorkflow.Shared.Logging
{
    /// <summary>
    /// Defines the mechanisms for a logger.
    /// </summary>
    public interface ILogger : IDisposable
    {
        /// <summary>
        /// Initializes the logger. Must be called first.
        /// </summary>
        /// <returns>Indicates if an error occured.</returns>
        bool Initialize();
        /// <summary>
        /// Writes some information.
        /// </summary>
        /// <param name="info">The information which will be loged.</param>
        void WriteInformation(string info);
        /// <summary>
        /// Writes some warning.
        /// </summary>
        /// <param name="warning">The warning which will be loged.</param>
        void WriteWarning(string warning);
        /// <summary>
        /// Writes some error.
        /// </summary>
        /// <param name="errorMessage">The error which will be loged.</param>
        void WriteError(string errorMessage);
    }
}
