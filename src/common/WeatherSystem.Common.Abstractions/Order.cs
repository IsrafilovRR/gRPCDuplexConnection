namespace WeatherSystem.Common.Abstractions;

public class Order
{
    public long Id { get; set; }
    public DateTime CreationDate { get; set; }
    public long ClientId { get; set; }
    public bool IsCompleted { get; set; }
    public int OrderTypeId { get; set; }
    public long WarehouseId { get; set; }
    public decimal Amount { get; set; }
    public Item[] ItemsData { get; set; }
}