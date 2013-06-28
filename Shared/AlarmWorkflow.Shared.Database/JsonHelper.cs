using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace AlarmWorkflow.Shared.Database
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
