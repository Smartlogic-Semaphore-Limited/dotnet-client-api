using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Smartlogic.Semaphore.Api.JSON
{
    
    internal class SesDateTimeConverter : DateTimeConverterBase
    {
        private const string FORMAT = "yyyyMMddHHmmss";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var convertor = new IsoDateTimeConverter {DateTimeFormat = FORMAT};
            convertor.WriteJson(writer,value,serializer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var convertor = new IsoDateTimeConverter {DateTimeFormat = FORMAT};
            return convertor.ReadJson(reader,objectType, existingValue, serializer);
        }
    }
}