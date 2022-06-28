using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Logging;
using WeatherSystem.Common.Abstractions;
using Order = WeatherSystem.Common.DataAccess.Entities.Order;

namespace WeatherSystem.Common.DataAccess.Repositories
{
    /// <inheritdoc cref="WeatherSystem.Common.DataAccess.Repositories.IOrderRepository" />
    public class OrderRepository : RepositoryBase, IOrderRepository
    {
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(IDbConnectionFactory connectionFactory, ILogger<OrderRepository> logger) :
            base(connectionFactory)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Abstractions.Order> GetOrdersAsync(long warehouseId, int orderTypeId,
            DateTime startDateTime,
            DateTime endDateTime)
        {
            if (startDateTime > endDateTime || warehouseId < 1 || orderTypeId < 1)
            {
                _logger.LogError("Incorrect arguments");
                throw new ArgumentNullException();
            }

            const string sql = @"
SELECT * FROM orders 
WHERE warehouseId = @warehouseId and orderTypeId = @orderTypeId 
  and creationDate >= @startDateTime and creationDate <= @endDateTime";

            await using var connection = await ConnectionFactory.CreateDbConnectionAsync();
            var reader = await connection.ExecuteReaderAsync(sql, new
            {
                warehouseId,
                orderTypeId,
                startDateTime,
                endDateTime
            });

            Func<IDataReader, Order> rowParser = reader.GetRowParser<Order>();
            while (await reader.ReadAsync())
            {
                var orderDto = rowParser(reader);
                yield return new Abstractions.Order
                {
                    Id = orderDto.Id,
                    Amount = orderDto.Amount,
                    ClientId = orderDto.ClientId,
                    CreationDate = orderDto.CreationDate,
                    IsCompleted = orderDto.IsCompleted,
                    OrderTypeId = orderDto.OrderTypeId,
                    WarehouseId = orderDto.WarehouseId,
                    ItemsData = JsonSerializer.Deserialize<Item[]>(orderDto.ItemsData)!
                };
            }
        }

        /// <inheritdoc />
        public async Task SaveOrdersAsync(Abstractions.Order[] orders)
        {
            var scripts = GetOrdersInsertSqlScriptsDividedOnBatches(orders).ToArray();
            _logger.LogDebug(
                $"Saving of the orders (count {orders.Length}) divided to batches(count {scripts.Length})");

            await using var connection = await ConnectionFactory.CreateDbConnectionAsync();
            foreach (var script in scripts)
            {
                await connection.ExecuteAsync(script);
            }
        }

        /// <summary>
        /// Get scripts for insertion the orders by batches 
        /// </summary>
        private static IEnumerable<string> GetOrdersInsertSqlScriptsDividedOnBatches(
            ICollection<Abstractions.Order> userNames)
        {
            const int batchSize = 3;
            const string insertSqlScript = @"
INSERT INTO orders(clientId, isCompleted, warehouseId, amount, orderTypeId, itemsData) VALUES ";

            var valuesSqlScript = "({0},{1},{2},{3},{4},'{5}')";
            var numberOfBatches = (int)Math.Ceiling((double)userNames.Count / batchSize);
            var result = new List<string>(numberOfBatches);

            for (var i = 0; i < numberOfBatches; i++)
            {
                var ordersToInsert = userNames
                    .Skip(i * batchSize)
                    .Take(batchSize);

                var valuesToInsert = ordersToInsert
                    .Select(order => string.Format(valuesSqlScript, order.ClientId, order.IsCompleted,
                        order.WarehouseId, order.Amount, order.OrderTypeId, JsonSerializer.Serialize(order.ItemsData)));

                result.Add(insertSqlScript + string.Join(',', valuesToInsert));
            }

            return result;
        }
    }
}