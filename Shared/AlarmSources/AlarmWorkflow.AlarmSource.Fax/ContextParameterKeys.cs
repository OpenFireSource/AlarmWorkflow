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


namespace AlarmWorkflow.AlarmSource.Fax
{
    /// <summary>
    /// Provides the key names of context parameters used by the fax alarm source.
    /// These can be used by jobs and/or other clients that are interested in that data.
    /// </summary>
    public static class ContextParameterKeys
    {
        /// <summary>
        /// Represents the context parameter key for the location of the archived file.
        /// This public static field is read-only.
        /// </summary>
        public static readonly string ArchivedFilePath = "ArchivedFilePath";
        /// <summary>
        /// Represents the context parameter key for the location of the currently processed image file.
        /// This public static field is read-only.
        /// </summary>
        public static readonly string ImagePath = "ImagePath";
    }
}
