using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
namespace KafkaHomework.OrderEventConsumer.Presentation.Converters;



public class WarehouseIdJsonConverter : JsonConverter<WarehouseId>
{
    public override WarehouseId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new WarehouseId(reader.GetInt64());  
    }

    public override void Write(Utf8JsonWriter writer, WarehouseId warehouseId, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(warehouseId.Value); 
    }
}