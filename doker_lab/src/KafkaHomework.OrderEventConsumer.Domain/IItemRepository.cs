using System.Threading.Tasks;
using KafkaHomework.OrderEventConsumer.Domain.Order;
namespace KafkaHomework.OrderEventConsumer.Domain;

public interface IItemRepository
{
    Task ReserveItems(OrderEvent orderEvent);
    Task SellItems(OrderEvent orderEvent);
    Task CancelItems(OrderEvent orderEvent);
    Task UpdateSellerPayments(OrderEvent orderEvent);
}
