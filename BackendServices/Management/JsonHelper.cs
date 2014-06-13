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

using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace AlarmWorkflow.BackendService.Management
{
    static class JsonHelper
    {
        internal static string ToJson(object graph)
        {
            if (graph == null)
            {
                return string.Empty;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(graph.GetType());
                serializer.WriteObject(stream, graph);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        internal static T FromJson<T>(string jsonString, T fallbackValue)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return fallbackValue;
            }

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }
    }
}