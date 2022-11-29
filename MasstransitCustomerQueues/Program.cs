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
            busRegistrationConfigurator.AddConsumer<TenantConsumer>();

            busRegistrationConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.Send<TenantMessage>(x => { x.UseRoutingKeyFormatter<TenantMessage>(z => z.Message.TenantId); });
                cfg.Publish<TenantMessage>(x => { x.ExchangeType = "direct"; });
            });
        });

        serviceCollection.AddHostedService<MessagePublisher>();
    });

host.Build().Run();