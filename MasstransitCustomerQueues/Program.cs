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
                
                cfg.Send<TenantMessage>(x =>
                {
                    x.UseRoutingKeyFormatter<TenantMessage>(z =>
                        z.Message.Aggregated ? "tenant-requests" : z.Message.TenantId);
                });

                cfg.Publish<TenantMessage>(x => { x.ExchangeType = "direct"; });
                cfg.ReceiveEndpoint("tenant-requests", xc =>
                {
                    xc.Bind<TenantMessage>(cb => cb.RoutingKey = "tenant-requests");
                    xc.Consumer<CommandExecutor>(ep => ep.ConcurrentMessageLimit = 1);
                });
            });
            busRegistrationConfigurator.AddRequestClient<TenantMessage>(RequestTimeout.After(m: 5));
        });

        serviceCollection.AddHostedService<MessagePublisher>();
    });

host.Build().Run();