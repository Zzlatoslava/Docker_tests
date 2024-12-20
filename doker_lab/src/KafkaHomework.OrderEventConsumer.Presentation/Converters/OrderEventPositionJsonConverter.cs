using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
using KafkaHomework.OrderEventConsumer.Domain.Order;
namespace KafkaHomework.OrderEventConsumer.Presentation.Converters;

public class OrderEventPositionJsonConverter : JsonConverter<OrderEventPosition>
{
    public override OrderEventPosition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var itemId = new ItemId(doc.RootElement.GetProperty("item_id").GetInt64());
            var quantity = doc.RootElement.GetProperty("quantity").GetInt32();
            var price = JsonSerializer.Deserialize<Money>(doc.RootElement.GetProperty("price").GetRawText(), options);
            return new OrderEventPosition(itemId, quantity, price);
        }
    }

    public override void Write(Utf8JsonWriter writer, OrderEventPosition value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("item_id", value.ItemId.Value);
        writer.WriteNumber("quantity", value.Quantity);
        writer.WritePropertyName("price");
        JsonSerializer.Serialize(writer, value.Price, options);
        writer.WriteEndObject();
    }
}
