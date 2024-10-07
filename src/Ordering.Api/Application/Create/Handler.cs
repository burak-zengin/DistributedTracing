using MediatR;
using Ordering.Api.Infrastructure.Persistence;

namespace Ordering.Api.Application.Create;

public class Handler(OrderingDbContext context) : IRequestHandler<Command, int>
{
    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var order = new Domain.Orders.Order
        {
            CreatedDateTime = DateTime.Now,
            Lines = request.Lines.Select(_ => new Domain.Orders.Line
            {
                Barcode = _.Barcode,
                Quantity = _.Quantity
            }).ToList()
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        return order.Id;
    }
}
