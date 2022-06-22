using Microsoft.AspNetCore.Mvc;
using WeatherSystem.Common.Abstractions;
using WeatherSystem.Common.DataAccess.Repositories;
using WeatherSystem.EventClient.Models.Requests;
using OrderDto = WeatherSystem.Common.Abstractions.Order;

namespace WeatherSystem.EventClient.Controllers;

/// <summary>
/// Orders controller
/// </summary>
/// <remarks>
/// this controller is not relevant for the project Weather system
/// it is used for testing purposes
/// </remarks>
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;

    public OrdersController(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    [HttpGet]
    public IAsyncEnumerable<OrderDto> GetOrders([FromQuery] long warehouseId, [FromQuery] int status,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        return _orderRepository.GetOrdersAsync(warehouseId, status, start, end);
    }

    [HttpPost]
    public async Task SaveOrders([FromBody] SaveOrdersRequestModel model)
    {
        await _orderRepository.SaveOrdersAsync(model.Orders.Select(order => new OrderDto
        {
            ClientId = order.ClientId,
            Amount = order.Amount,
            IsCompleted = order.IsCompleted,
            WarehouseId = order.WarehouseId,
            OrderTypeId = order.OrderTypeId,
            ItemsData = order.ItemsData.Select(orderItemModel => new Item
            {
                Id = orderItemModel.Id,
                Count = orderItemModel.Count
            }).ToArray()
        }).ToArray());
    }
}