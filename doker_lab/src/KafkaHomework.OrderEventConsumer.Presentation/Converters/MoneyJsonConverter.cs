using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
namespace KafkaHomework.OrderEventConsumer.Presentation.Converters;

public class MoneyJsonConverter : JsonConverter<Money>
{
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var currency = doc.RootElement.GetProperty("currency").GetString();
            if (string.IsNullOrEmpty(currency))
            {
                throw new JsonException("Currency cannot be null or empty.");
            }
            var units = doc.RootElement.GetProperty("units").GetInt64();
            var nanos = doc.RootElement.GetProperty("nanos").GetInt32();
            return new Money(units + nanos / 1_000_000_000m, currency);
        }
    }

    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("currency", value.Currency);
        writer.WriteNumber("units", (long)value.Value);
        writer.WriteNumber("nanos", (int)((value.Value - (long)value.Value) * 1_000_000_000));
        writer.WriteEndObject();
    }
}
