using WeatherSystem.Common.Abstractions;

namespace WeatherSystem.Common.DataAccess.Repositories
{
    /// <summary>
    /// Repository for <see cref="Abstractions.Order"/>
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Get orders by async enumerator
        /// </summary>
        /// <param name="warehouseId">warehouse id</param>
        /// <param name="orderType">order type id</param>
        /// <param name="startDateTime">start date time</param>
        /// <param name="endDateTime">end date time</param>
        IAsyncEnumerable<Order> GetOrdersAsync(long warehouseId, int orderType,
            DateTime startDateTime,
            DateTime endDateTime);

        /// <summary>
        /// Save orders
        /// </summary>
        /// <param name="orders">orders</param>
        Task SaveOrdersAsync(Order[] orders);
    }
}