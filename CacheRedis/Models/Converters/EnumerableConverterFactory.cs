using System.Text.Json.Serialization;
using System.Text.Json;

namespace CacheRedis.Models.Converters;
public class EnumerableConverterFactory(Type _interfaceType) : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (typeToConvert.Equals(typeof(IEnumerable<>).MakeGenericType(_interfaceType))
         && typeToConvert.GenericTypeArguments[0].Equals(_interfaceType))
        {
            return true;
        }
        return false;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type converterType = typeof(EnumerableConverter<>).MakeGenericType(_interfaceType);
        if (Activator.CreateInstance(converterType) is not JsonConverter jsonConverter)
        {
            throw new NullReferenceException("Ошибка создания конвертера Json");
        }
        return jsonConverter;
    }
}
