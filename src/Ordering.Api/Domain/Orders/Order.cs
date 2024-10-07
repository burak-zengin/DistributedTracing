namespace Ordering.Api.Domain.Orders;

public class Order
{
    public int Id { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public List<Line> Lines { get; set; }
}
