namespace Ordering.Api.Domain.Orders;

public class Line
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string Barcode { get; set; }

    public int Quantity { get; set; }
}
