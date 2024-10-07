using MediatR;

namespace Stock.Api.Application.Get;

public record Query : IRequest<List<Domain.Stocks.Stock>>;
