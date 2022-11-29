using MassTransit;
using MasstransitCustomerQueues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(serviceCollection =>
    {
        serviceCollection.AddLogging(loggerFactory => loggerFactory
            // .AddFilter("System", LogLevel.Debug)
            // .AddFilter("Microsoft", LogLevel.Debug)
            // .AddFilter("MassTransit", LogLevel.Debug)
            .AddConsole());


        serviceCollection.AddMassTransit(busRegistrationConfigurator =>
        {
            busRegistrationConfigurator.AddConsumer<TenantConsumer>(cfg =>
            {
                cfg.UseConcurrencyLimit(1);
                cfg.ConcurrentMessageLimit = 1;
            });

            busRegistrationConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint(
                    x => { x.Consumer<CommandExecutor>(cmCfg => { cmCfg.ConcurrentMessageLimit = 1; }); });

                cfg.Send<TenantMessage>(x => { x.UseRoutingKeyFormatter(z => z.Message.TenantId); });

                cfg.Publish<TenantMessage>(x => { x.ExchangeType = "direct"; });
            });
            busRegistrationConfigurator.AddRequestClient<ExecuteCommand>(RequestTimeout.After(m: 5));
        });

        serviceCollection.AddHostedService<MessagePublisher>();
    });

host.Build().Run();