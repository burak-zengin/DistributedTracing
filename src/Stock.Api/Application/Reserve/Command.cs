using MediatR;

namespace Stock.Api.Application.Reserve;

public record Command(List<Item> Stocks) : IRequest;
