using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock.Api.Infrastructure.Persistence;

namespace Stock.Api.Application.Get;

public class Handler(StockDbContext context) : IRequestHandler<Query, List<Domain.Stocks.Stock>>
{
    public async Task<List<Domain.Stocks.Stock>> Handle(Query request, CancellationToken cancellationToken)
    {
        var stocks = await context.Stocks.ToListAsync();

        return stocks;
    }
}
