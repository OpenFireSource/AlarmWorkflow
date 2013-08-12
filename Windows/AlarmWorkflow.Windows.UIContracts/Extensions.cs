// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Globalization;

namespace AlarmWorkflow.Windows.UIContracts
{
    /// <summary>
    /// Provides extensions for the UI.
    /// </summary>
    public static class ClientExtensions
    {
        /// <summary>
        /// Returns the Uri using the calling assembly and the given path.
        /// </summary>
        /// <param name="source">The instance that this method is called on. Used to infer assembly name for resource retrieval.</param>
        /// <param name="path">The path. This is the text that follows the "component/" text.</param>
        /// <returns></returns>
        public static Uri GetPackUri(this object source, string path)
        {
            string applicationName = source.GetType().Assembly.GetName().Name;
            return GetPackUri(applicationName, path);
        }

        /// <summary>
        /// Returns the Uri using the given application name and the given path.
        /// </summary>
        /// <param name="applicationName">The application name. This is the name of the assembly.</param>
        /// <param name="path">The path. This is the text that follows the "component/" text.</param>
        /// <returns></returns>
        public static Uri GetPackUri(string applicationName, string path)
        {
            return new Uri(string.Format(CultureInfo.InvariantCulture, "pack://application:,,,/{0};component/{1}", applicationName, path));

        }
    }
}