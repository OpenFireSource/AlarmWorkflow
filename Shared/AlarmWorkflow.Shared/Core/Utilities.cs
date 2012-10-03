using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using AlarmWorkflow.Shared.Diagnostics;

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

        /// <summary>
        /// Analyzes a TIFF-file, extracts all pages (1..n) to a bitmap file, and returns the full file name for each.
        /// </summary>
        /// <param name="tiffFileName"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetMergedTifFileNames(string tiffFileName)
        {
            // Idea taken from http://code.msdn.microsoft.com/windowsdesktop/Split-multi-page-tiff-file-058050cc
            // Sorry for the weird method name... any better ideas?

            //Get the frame dimension list from the image of the file and 
            using (Image tiffImage = Image.FromFile(tiffFileName))
            {
                //get the globally unique identifier (GUID) 
                Guid objGuid = tiffImage.FrameDimensionsList[0];
                //create the frame dimension 
                FrameDimension dimension = new FrameDimension(objGuid);
                //Gets the total number of frames in the .tiff file 
                int noOfPages = tiffImage.GetFrameCount(dimension);

                foreach (Guid guid in tiffImage.FrameDimensionsList)
                {
                    for (int index = 0; index < noOfPages; index++)
                    {
                        FrameDimension currentFrame = new FrameDimension(guid);
                        tiffImage.SelectActiveFrame(currentFrame, index);

                        string fileName = Path.Combine(Path.GetDirectoryName(tiffFileName), Path.GetFileNameWithoutExtension(tiffFileName) + index + ".bmp");
                        tiffImage.Save(fileName, ImageFormat.Bmp);
                        yield return fileName;
                    }
                }
            }
        }

        /// <summary>
        /// Trims all lines from the array, which means that lines with zero length will be omitted.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static string[] Trim(string[] lines)
        {
            List<string> nl = new List<string>(lines.Length);
            foreach (string item in lines)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    nl.Add(item);
                }
            }
            return nl.ToArray();
        }

        /// <summary>
        /// Executes a delegate and swallows (ignores) all exceptions. Useful in scenarios where throwing exceptions are no option due to stability.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="parameter">An optional parameter.</param>
        public static void Swallow<T>(Action<T> action, T parameter)
        {
            try
            {
                action(parameter);
            }
            catch (Exception ex)
            {
                // However still log this exception
                Logger.Instance.LogFormat(LogType.Exception, "Utilities.Swallow", "An exception was swallowed while executing a delegate. The process will continue. See exception details following.");
                Logger.Instance.LogException("Utilities.Swallow", ex);
            }
        }
    }
}