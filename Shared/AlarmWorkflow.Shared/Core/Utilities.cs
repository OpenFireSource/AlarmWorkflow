using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

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
                        MemoryStream ms = new MemoryStream();
                        tiffImage.Save(ms, tiffImage.RawFormat);
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = ms;
                        image.EndInit();

                        if (tiffFileName != null)
                        {
                            string fileName = Path.Combine(Path.GetDirectoryName(tiffFileName), Path.GetFileNameWithoutExtension(tiffFileName) + index + ".tiff");
                            FileStream stream = new FileStream(fileName, FileMode.Create);
                            TiffBitmapEncoder encoder = new TiffBitmapEncoder { Compression = TiffCompressOption.Zip };
                            encoder.Frames.Add(BitmapFrame.Create(image));
                            encoder.Save(stream);
                            stream.Flush(true);
                            stream.Close();

                            yield return fileName;
                        }
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

        /// <summary>
        /// Returns the full path name of the directory that shall store all user-specific application data.
        /// </summary>
        /// <returns></returns>
        public static string GetLocalAppDataFolderPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenFireSource", "AlarmWorkflow");
        }

        /// <summary>
        /// Returns the full name of a file that is located within the directory that shall store all user-specific application data.
        /// </summary>
        /// <returns></returns>
        public static string GetLocalAppDataFolderFileName(string fileName)
        {
            return Path.Combine(GetLocalAppDataFolderPath(), fileName);
        }

        /// <summary>
        /// Truncates the given string. Ensures a string has a maximum <paramref name="length"/> and cuts away following chars,
        /// optionally adding ellipsis to the end.
        /// </summary>
        /// <param name="value">The string to truncate.</param>
        /// <param name="length">The truncated length (including ellispis, if chosen).</param>
        /// <param name="leftAlign"><c>true</c> to align from the left (default) and add ellipsis to the right, <c>false</c> to invert.</param>
        /// <param name="addEllipsis">Whether or not to add ellpsis to the truncated string. This only applies if the string is longer than the desired maximum length.</param>
        /// <returns>The truncated string.</returns>
        public static string Truncate(this string value, int length, bool leftAlign, bool addEllipsis)
        {
            string ret = value;
            // add ellipsis?
            if (addEllipsis) { length -= 3; }

            if (value.Length > length)
            {
                if (leftAlign)
                {
                    ret = ret.Substring(0, length);
                    // add ellipsis?
                    if (addEllipsis) { ret += "..."; }
                }
                else
                {
                    ret = ret.Remove(0, value.Length - length);
                    if (addEllipsis) { ret = ret.Insert(0, "..."); }
                }
            }
            return ret;
        }
    }
}