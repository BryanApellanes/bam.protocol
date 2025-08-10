
using Newtonsoft.Json;

namespace Bam.Protocol.Data;

public class InterfaceTypeConverter<TInterface, TConcrete> : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(TInterface));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return serializer.Deserialize(reader, typeof(TConcrete));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value, typeof(TConcrete));
    }
}