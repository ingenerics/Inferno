using System;
using System.IO;
using System.Text;

namespace Inferno.Testing
{
    public static class JsonHelper
    {
        public static string Serialize<T>(T serializeObject)
        {
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(serializeObject.GetType());
            var ms = new MemoryStream();
            serializer.WriteObject(ms, serializeObject);
            return Encoding.Default.GetString(ms.ToArray());
        }

        public static T Deserialize<T>(string json)
        {
            var obj = Activator.CreateInstance<T>();
            var ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }
}
