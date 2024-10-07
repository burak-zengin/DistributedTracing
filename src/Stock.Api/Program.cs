using Microsoft.EntityFrameworkCore;
using Stock.Api.Infrastructure.Persistence;
using MassTransit;
using Stock.Api.Consumers;
using Microsoft.Extensions.Options;
using MediatR;
using Stock.Api.Application.Get;
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
builder.Services.AddDbContext<StockDbContext>(options =>
{
    options.UseInMemoryDatabase("Stock");
});
builder
    .Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Stock.Api"))
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
    configure.AddConsumer<OrderCreatedEventConsumer>();

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

app.MapGet("/", async (
    IMediator mediator) =>
{
    return await mediator.Send(new Query());
})
.WithOpenApi();

app.Run();