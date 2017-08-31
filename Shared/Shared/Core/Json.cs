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

using Newtonsoft.Json;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Provides functionality to work with JSON (JavaScript Object Notation).
    /// </summary>
    public static class Json
    {
        #region Methods

        /// <summary>
        /// Serializes the given object instance into its appropriate JSON-representation.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="ignoreNullValue">Ignore all null values in the object.</param>
        /// <returns>A string containing the JSON-serialized object instance.</returns>
        public static string Serialize(object value, bool ignoreNullValue = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            if (ignoreNullValue)
            {
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            }

            return JsonConvert.SerializeObject(value, settings);
        }

        /// <summary>
        /// Deserializes the given JSON-string back into an object.
        /// </summary>
        /// <param name="value">The JSON-string to deserialize.</param>
        /// <param name="ignoreNullValue">Ignore all null values in the object.</param>
        /// <returns>The deserialized object instance.</returns>
        public static T Deserialize<T>(string value, bool ignoreNullValue = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            if (ignoreNullValue)
            {
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            }

            return JsonConvert.DeserializeObject<T>(value, settings);
        }

        #endregion
    }
}
