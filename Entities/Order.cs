public class Order : Entity
{
    public string Customer { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    //  public List<OrderItem> Items { get; set; }
}