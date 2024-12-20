using KafkaHomework.OrderEventConsumer.Domain;
using System.Threading.Tasks;
using KafkaHomework.OrderEventConsumer.Domain.Order;
using Npgsql;

namespace KafkaHomework.OrderEventConsumer.Infrastructure;

public sealed class ItemRepository : IItemRepository
{
    private readonly string _connectionString;

    public ItemRepository(string connectionString) => _connectionString = connectionString;

    public async Task ReserveItems(OrderEvent orderEvent)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new NpgsqlCommand(@"
            insert into Orders (item_id, quantity_reserved, last_updated, quantity_sold, quantity_canceled ) 
            values (@ItemId, @Quantity, @LastUpdated, 0, 0)
            ON CONFLICT (item_id) DO NOTHING
"
            , connection);

        foreach (var position in orderEvent.Positions)
        {
            command.Parameters.AddWithValue("Quantity", position.Quantity);
            command.Parameters.AddWithValue("LastUpdated", orderEvent.Moment);
            command.Parameters.AddWithValue("ItemId", position.ItemId.Value);
            
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task SellItems(OrderEvent orderEvent)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new NpgsqlCommand(@"
        update Orders 
           set quantity_sold = quantity_sold + @Quantity
               ,last_updated = @LastUpdated 
         where item_id = @ItemId
", connection);

        foreach (var position in orderEvent.Positions)
        {
            command.Parameters.AddWithValue("Quantity", position.Quantity);
            command.Parameters.AddWithValue("ItemID", position.ItemId.Value);
            command.Parameters.AddWithValue("LastUpdated", orderEvent.Moment);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task CancelItems(OrderEvent orderEvent)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new NpgsqlCommand(@"
            update Orders 
               set quantity_canceled = quantity_canceled + @Quantity
                   ,last_updated = @LastUpdated 
             where item_id = @ItemId
", connection);

        foreach (var position in orderEvent.Positions)
        {
            command.Parameters.AddWithValue("Quantity", position.Quantity);
            command.Parameters.AddWithValue("ItemID", position.ItemId.Value);
            command.Parameters.AddWithValue("LastUpdated", orderEvent.Moment);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task UpdateSellerPayments(OrderEvent orderEvent)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
    
        foreach (var position in orderEvent.Positions)
        {
            string itemIdString = position.ItemId.Value.ToString();
            long sellerId = long.Parse(itemIdString.Substring(0, 6)); 
            long productId = long.Parse(itemIdString.Substring(6, 6)); 
        
            decimal totalSales = position.Price.Value;
        
            var command = new NpgsqlCommand(@"
                insert into seller_payments (seller_id, product_id, currency, total_sales, quantity_sold)
                values (@SellerId, @ProductId, @Currency, @TotalSales, @QuantitySold)
                on conflict (seller_id, product_id, currency)
                do update
                set total_sales = seller_payments.total_sales +  @TotalSales,
                    quantity_sold = seller_payments.quantity_sold + @QuantitySold
        ", connection);

            command.Parameters.AddWithValue("SellerId", sellerId);
            command.Parameters.AddWithValue("ProductId", productId);
            command.Parameters.AddWithValue("Currency", position.Price.Currency);
            command.Parameters.AddWithValue("TotalSales", totalSales);
            command.Parameters.AddWithValue("QuantitySold", position.Quantity);

            await command.ExecuteNonQueryAsync();
        }

    }
}
