using Microsoft.EntityFrameworkCore;
using Ordering.Api.Infrastructure.Persistence;
using MassTransit;
using Microsoft.Extensions.Options;
using MediatR;
using Ordering.Api.Application.Create;
using Shared.Events;
using System.Reflection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    setup.CustomSchemaIds(_ => _.FullName);
});
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});
builder.Services.AddDbContext<OrderingDbContext>(options =>
{
    options.UseInMemoryDatabase("Ordering");
});
builder
    .Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Ordering.Api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSqlClientInstrumentation(o => o.SetDbStatementForText = true)
            .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);

        tracing.AddOtlpExporter();
    });
builder.Services.Configure<Shared.Settings.Masstransit>(builder.Configuration.GetSection("Masstransit"));
builder.Services.AddMassTransit(configure =>
{
    configure.UsingRabbitMq((context, configure) =>
    {
        var options = context.GetRequiredService<IOptions<Shared.Settings.Masstransit>>();

        configure.Host(options.Value.RabbitMq.Host, configure =>
        {
            configure.Username(options.Value.RabbitMq.Username);
            configure.Password(options.Value.RabbitMq.Password);
        });

        configure.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapPost("/", async (
    IMediator mediator,
    Command command,
    ISendEndpointProvider sendEndpointProvider) =>
{
    var response = await mediator.Send(command);

    var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:OrderCreatedEvent"));
    await sendEndpoint.Send(
        new OrderCreatedEvent(
            new Order(
                response,
                command
                    .Lines
                    .Select(_ => new Shared.Events.Line(
                        _.Barcode,
                        _.Quantity))
                    .ToList())));

    return response;
})
.WithOpenApi();

app.Run();
