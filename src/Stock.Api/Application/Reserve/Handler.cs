using MediatR;
using Stock.Api.Infrastructure.Persistence;

namespace Stock.Api.Application.Reserve;

public class Handler(StockDbContext context) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        foreach (var stock in request.Stocks)
        {
            context.Stocks.First(_ => _.Barcode == stock.Barcode).Quantity -= stock.Reserve;
        }

        await context.SaveChangesAsync();
    }
}
