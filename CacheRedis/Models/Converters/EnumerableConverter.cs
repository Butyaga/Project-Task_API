using System.Text.Json.Serialization;
using System.Text.Json;

namespace CacheRedis.Models.Converters;
class EnumerableConverter<M> : JsonConverter<IEnumerable<M>>
{
    public override IEnumerable<M>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<List<M>>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, IEnumerable<M> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Только для десериализации");
    }
}
