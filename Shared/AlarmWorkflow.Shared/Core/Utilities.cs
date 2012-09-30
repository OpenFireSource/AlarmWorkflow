using System.Collections.Generic;
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

        /// <summary>
        /// Returns the first instance if available or the default value for T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static T FirstOrDefault<T>(IEnumerable<T> enumerable)
        {
            IEnumerator<T> enumerator = enumerable.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }
            return default(T);
        }

    }
}