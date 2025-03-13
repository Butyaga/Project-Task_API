using System.Text.Json.Serialization;
using System.Text.Json;

namespace CacheRedis.Models.Converters;
public class InterfaceConverterFactory(Type _concreteType, Type _interfaceType) : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == _interfaceType;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type converterType = typeof(InterfaceConverter<,>).MakeGenericType(_concreteType, _interfaceType);
        if (Activator.CreateInstance(converterType) is not JsonConverter jsonConverter)
        {
            throw new NullReferenceException("Ошибка создания конвертера Json");
        }
        return jsonConverter;
    }
}
