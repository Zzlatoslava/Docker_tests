using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
namespace KafkaHomework.OrderEventConsumer.Presentation.Converters;


public class UserIdJsonConverter : JsonConverter<UserId>
{
    public override UserId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new UserId(reader.GetInt64());  
    }

    public override void Write(Utf8JsonWriter writer, UserId userId, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(userId.Value); 
    }
}