using MediatR;

namespace Ordering.Api.Application.Create;

public record Command(List<Line> Lines) : IRequest<int>;
