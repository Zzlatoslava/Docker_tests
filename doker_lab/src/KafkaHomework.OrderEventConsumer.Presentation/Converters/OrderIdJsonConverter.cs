using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
namespace KafkaHomework.OrderEventConsumer.Presentation.Converters;



public class OrderIdJsonConverter : JsonConverter<OrderId>
{
    public override OrderId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new OrderId(reader.GetInt64());  
    }

    public override void Write(Utf8JsonWriter writer, OrderId orderId, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(orderId.Value); 
    }
}
