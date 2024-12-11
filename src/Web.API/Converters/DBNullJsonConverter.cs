using System.Text.Json.Serialization;
using System.Text.Json;

namespace Web.API.Converters;
public sealed class DBNullJsonConverter : JsonConverter<DBNull>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(DBNull);
    }

    public override DBNull Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, DBNull value, JsonSerializerOptions options)
    {
        writer.WriteNullValue();
    }
}
