using System.IO;
using System.Reflection;
using System.Text;

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

        /// <summary>
        /// Combines many paths.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string Combine(params string[] paths)
        {
            string path = "";
            for (int i = 0; i < paths.Length; i++)
            {
                path = Path.Combine(path, paths[i]);
            }
            return path;
        }
    }
}