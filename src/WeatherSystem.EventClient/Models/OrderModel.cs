namespace WeatherSystem.EventClient.Models;

public class OrderModel
{
    public long ClientId { get; set; }
    public bool IsCompleted { get; set; }
    public int OrderTypeId { get; set; }
    public long WarehouseId { get; set; }
    public decimal Amount { get; set; }
    public OrderItemModel[] ItemsData { get; set; }
}