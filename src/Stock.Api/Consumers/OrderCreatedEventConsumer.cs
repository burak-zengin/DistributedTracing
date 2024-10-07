using MassTransit;
using MediatR;
using Shared.Events;
using Stock.Api.Application.Reserve;

namespace Stock.Api.Consumers;

public class OrderCreatedEventConsumer(IMediator mediator) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        await mediator
            .Send(new Command(
                context
                    .Message
                    .Order
                    .Lines
                    .Select(_ => new Item(_.Barcode, _.Quantity))
                    .ToList()));
    }
}
