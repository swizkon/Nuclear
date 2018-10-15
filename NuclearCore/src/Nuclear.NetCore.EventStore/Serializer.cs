using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nuclear.NetCore.EventStore
{
    internal static class Serializer
    {
        private const string EventClrTypeHeader = "EventClrTypeName";
        private const int WritePageSize = 500;
        private const int ReadPageSize = 500;

        public static object DeserializeEvent(byte[] metadata, byte[] data)
        {
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata)).Property(EventClrTypeHeader).Value;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType((string)eventClrTypeName));
        }
    }
}
