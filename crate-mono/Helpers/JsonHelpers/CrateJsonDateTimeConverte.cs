using System;
using Crate.Exceptions;
using Newtonsoft.Json;

namespace Crate.Helpers.JsonHelpers
{
    public class CrateJsonDateTimeConverte : JsonConverter
    {
        private readonly DateTime _unixDt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is DateTime))
                throw new CrateException(string.Format("Date expected {0}", writer.Path));

            var diff = ((DateTime)value) - _unixDt;
            var ticks = (long)diff.TotalSeconds;
            writer.WriteValue(ticks);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Integer)
            {
                throw new CrateException(string.Format("Integer expected when parsing date field: {0}.", reader.Path));
            }

            var ticks = (long)reader.Value;

            return _unixDt.AddMilliseconds(ticks);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }
    }
}