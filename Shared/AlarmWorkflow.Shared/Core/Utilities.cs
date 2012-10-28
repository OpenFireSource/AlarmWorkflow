using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Contains miscellaneous common functionality.
    /// </summary>
    public static class Utilities
    {
        #region Fields

        private static readonly BinaryFormatter Formatter;

        #endregion

        #region Constructors

        static Utilities()
        {
            Formatter = new BinaryFormatter();
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Serializes a graph into a byte-array using binary serialization.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static byte[] Serialize(object graph)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Formatter.Serialize(stream, graph);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserializes a binary-serialized object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] buffer)
        {
            if (buffer == null)
            {
                return default(T);
            }

            using (MemoryStream stream = new MemoryStream(buffer))
            {
                return (T)Formatter.Deserialize(stream);
            }
        }

        #endregion

        #region XDocument

        /// <summary>
        /// Tries to get the element value of an element from the requested XElement.
        /// </summary>
        /// <param name="element">The XElement to get the element value from.</param>
        /// <param name="elementName">The name of the element to get its value.</param>
        /// <param name="defaultValue">The default value to return, if the element did not exist.</param>
        /// <returns></returns>
        public static string TryGetElementValue(this XElement element, string elementName, string defaultValue)
        {
            XElement e = element.Element(elementName);
            if (e != null)
            {
                return e.Value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Tries to get the bool element value of an element from the requested XElement.
        /// </summary>
        /// <param name="element">The XElement to get the element value from.</param>
        /// <param name="elementName">The name of the element to get its value.</param>
        /// <param name="defaultValue">The default value to return, if the element did not exist.</param>
        /// <returns></returns>
        public static bool TryGetElementValue(this XElement element, string elementName, bool defaultValue)
        {
            string value = TryGetElementValue(element, elementName, defaultValue.ToString());
            bool returnValue = false;
            bool.TryParse(value, out returnValue);
            return returnValue;
        }

        /// <summary>
        /// Tries to get the int element value of an element from the requested XElement.
        /// </summary>
        /// <param name="element">The XElement to get the element value from.</param>
        /// <param name="elementName">The name of the element to get its value.</param>
        /// <param name="defaultValue">The default value to return, if the element did not exist.</param>
        /// <returns></returns>
        public static int TryGetElementValue(this XElement element, string elementName, int defaultValue)
        {
            string value = TryGetElementValue(element, elementName, defaultValue.ToString());
            int returnValue = -1;
            int.TryParse(value, out returnValue);
            return returnValue;
        }

        /// <summary>
        /// Tries to get the float element value of an element from the requested XElement.
        /// </summary>
        /// <param name="element">The XElement to get the element value from.</param>
        /// <param name="elementName">The name of the element to get its value.</param>
        /// <param name="defaultValue">The default value to return, if the element did not exist.</param>
        /// <returns></returns>
        public static float TryGetElementValue(this XElement element, string elementName, float defaultValue)
        {
            string value = TryGetElementValue(element, elementName, defaultValue.ToString());
            float returnValue = -1.0f;
            float.TryParse(value, out returnValue);
            return returnValue;
        }

        /// <summary>
        /// Tries to get the double element value of an element from the requested XElement.
        /// </summary>
        /// <param name="element">The XElement to get the element value from.</param>
        /// <param name="elementName">The name of the element to get its value.</param>
        /// <param name="defaultValue">The default value to return, if the element did not exist.</param>
        /// <returns></returns>
        public static double TryGetElementValue(this XElement element, string elementName, double defaultValue)
        {
            string value = TryGetElementValue(element, elementName, defaultValue.ToString());
            double returnValue = -1.0f;
            double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out returnValue);
            return returnValue;
        }

        /// <summary>
        /// Tries to get the attribute value of an attribute from the requested XElement.
        /// </summary>
        /// <param name="element">The XElement to get the attribute value from.</param>
        /// <param name="attributeName">The name of the attribute to get its value.</param>
        /// <param name="defaultValue">The default value to return, if the attribute did not exist.</param>
        /// <returns></returns>
        public static string TryGetAttributeValue(this XElement element, string attributeName, string defaultValue)
        {
            XAttribute attribute = element.Attribute(attributeName);
            if (attribute != null)
            {
                return attribute.Value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Tries to get the boolean element value of an element from the requested XElement.
        /// </summary>
        /// <param name="element">The XElement to get the element value from.</param>
        /// <param name="attributeName">The name of the element to get its value.</param>
        /// <param name="defaultValue">The default value to return, if the element did not exist.</param>
        /// <returns></returns>
        public static bool TryGetAttributeValue(this XElement element, string attributeName, bool defaultValue)
        {
            string value = TryGetAttributeValue(element, attributeName, defaultValue.ToString());
            bool returnValue = false;
            bool.TryParse(value, out returnValue);
            return returnValue;
        }

        /// <summary>
        /// Tries to get the integer element value of an element from the requested XElement.
        /// </summary>
        /// <param name="element">The XElement to get the element value from.</param>
        /// <param name="attributeName">The name of the element to get its value.</param>
        /// <param name="defaultValue">The default value to return, if the element did not exist.</param>
        /// <returns></returns>
        public static int TryGetAttributeValue(this XElement element, string attributeName, int defaultValue)
        {
            string value = TryGetAttributeValue(element, attributeName, defaultValue.ToString());
            int returnValue = -1;
            int.TryParse(value, out returnValue);
            return returnValue;
        }

        /// <summary>
        /// Tries to get the float element value of an element from the requested XElement.
        /// </summary>
        /// <param name="element">The XElement to get the element value from.</param>
        /// <param name="attributeName">The name of the element to get its value.</param>
        /// <param name="defaultValue">The default value to return, if the element did not exist.</param>
        /// <returns></returns>
        public static float TryGetAttributeValue(this XElement element, string attributeName, float defaultValue)
        {
            string value = TryGetAttributeValue(element, attributeName, defaultValue.ToString());
            float returnValue = -1.0f;
            float.TryParse(value, out returnValue);
            return returnValue;
        }

        /// <summary>
        /// Looks up whether or not an element with the given name exists in the list of elements and returns its value then.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementName">The name of the element to get its value.</param>
        /// <param name="defaultValue">The default value to return if the element didn't exist.</param>
        /// <returns></returns>
        public static string TryGetElementValue(this IList<XElement> elements, string elementName, string defaultValue)
        {
            var el = elements.FirstOrDefault(e => string.Equals(e.Name.LocalName, elementName, StringComparison.InvariantCulture));
            if (el != null)
            {
                return el.Value;
            }
            return defaultValue;
        }

        #endregion

        #region Resources Utilities

        /// <summary>
        /// Looks for a file with build action set to "Embedded resource" in a given assembly
        /// and returns its contents as a string.
        /// </summary>
        /// <param name="assembly">The assembly to get the embedded resource from.</param>
        /// <param name="resourceName">The resource name. This name is case-sensitive!</param>
        /// <returns>The contents of the embedded resource file as a string.</returns>
        public static string GetEmbeddedResourceText(this Assembly assembly, string resourceName)
        {
            try
            {
                string fullResourceName = assembly.GetName().Name + "." + resourceName;

                Stream stream = assembly.GetManifestResourceStream(fullResourceName);
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        #endregion

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
        /// Returns the working directory of this assembly.
        /// </summary>
        /// <returns>The working directory of this assembly.</returns>
        public static string GetWorkingDirectory()
        {
            return GetWorkingDirectory(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Gets the absolute path from the relative path.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <returns></returns>
        public static string GetAbsolutePath(string relativePath)
        {
            return Path.Combine(GetWorkingDirectory(), relativePath);
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

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the service to return.</typeparam>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/>-instance to get the service from.</param>
        /// <returns>The service object of the specified type.</returns>
        /// <exception cref="System.InvalidOperationException">The specified service could not be found.</exception>
        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }
    }
}