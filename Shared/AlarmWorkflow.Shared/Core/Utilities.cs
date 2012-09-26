using System.IO;
using System.Reflection;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Contains miscellaneous common functionality.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Returns the working directory of this assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The working directory of this assembly.</returns>
        public static string GetWorkingDirectory(Assembly assembly)
        {
            return Path.GetDirectoryName(assembly.Location);
        }
    }
}