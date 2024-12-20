using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using KafkaHomework.OrderEventConsumer.Infrastructure.Kafka;
using System.Text.Json;
using System.Text.Json.Serialization;
using KafkaHomework.OrderEventConsumer.Domain.Order;
using KafkaHomework.OrderEventConsumer.Domain;
using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
using KafkaHomework.OrderEventConsumer.Presentation.Converters;

namespace KafkaHomework.OrderEventConsumer.Presentation;

public class ItemHandler : IHandler<Ignore, string>
{
    private readonly ILogger<ItemHandler> _logger;
    private readonly IItemRepository _repository;
    private readonly Random _random = new();

    public ItemHandler(ILogger<ItemHandler> logger, IItemRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task Handle(IReadOnlyCollection<ConsumeResult<Ignore, string>> messages, CancellationToken token)
    {
        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new OrderIdJsonConverter(),
                new UserIdJsonConverter(),
                new WarehouseIdJsonConverter(),
                new JsonStringEnumConverter(),
                new MoneyJsonConverter(),
                new OrderEventPositionJsonConverter(),
            },
           PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
        };
        
        foreach (var message in messages)
        {
            try
            {
                var orderEvent = JsonSerializer.Deserialize<OrderEvent>(message.Message.Value, options);
               
                if (orderEvent != null )
                {
                    if (orderEvent.Status == Status.Created)
                    {
                        await _repository.ReserveItems(orderEvent);
                    }
                    else if (orderEvent.Status == Status.Delivered)
                    {
                        await _repository.SellItems(orderEvent);
                        await _repository.UpdateSellerPayments(orderEvent);
                    }
                    else if (orderEvent.Status == Status.Cancelled)
                    {
                        await _repository.CancelItems(orderEvent);
                    }
                    _logger.LogInformation("Processed order with ID {OrderId}\n{Message}", orderEvent.OrderId.Value,message.Message.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing message: {Message}", message.Message.Value);
            }
        }

        _logger.LogInformation("Handled {Count} messages", messages.Count);
    }
}
