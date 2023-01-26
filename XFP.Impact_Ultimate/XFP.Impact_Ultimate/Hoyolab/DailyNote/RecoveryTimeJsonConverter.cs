using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace XFP.Impact_Ultimate.Hoyolab.DailyNote
{
    public class RecoveryTimeJsonConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var secondStr = reader.GetString();
            if (int.TryParse(secondStr, out var second))
            {
                return TimeSpan.FromSeconds(second);
            }
            else
            {
                return TimeSpan.Zero;
            }
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.TotalSeconds.ToString());
        }
    }
}
