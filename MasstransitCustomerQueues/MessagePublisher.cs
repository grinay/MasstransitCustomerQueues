using MassTransit;
using MassTransit.RabbitMqTransport.Configuration;
using Microsoft.Extensions.Hosting;

namespace MasstransitCustomerQueues;

public class MessagePublisher : IHostedService
{
    private readonly IBus _bus;
    private readonly IServiceProvider _serviceProvider;

    public MessagePublisher(IBus bus, IServiceProvider serviceProvider)
    {
        _bus = bus;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _bus.ConnectReceiveEndpoint("tenant1", e =>
        {
            var mqCfg = (RabbitMqReceiveEndpointConfiguration)e;
            mqCfg.Bind<TenantMessage>(cb => { cb.RoutingKey = "tenant1"; });
            mqCfg.Consumer<TenantConsumer>(_serviceProvider, cfg => cfg.ConcurrentMessageLimit = 1);
        });

        _bus.ConnectReceiveEndpoint("tenant2", e =>
        {
            var mqCfg = e as RabbitMqReceiveEndpointConfiguration;
            mqCfg.Bind<TenantMessage>(cb => { cb.RoutingKey = "tenant2"; });
            mqCfg.Consumer<TenantConsumer>(_serviceProvider, cfg => cfg.ConcurrentMessageLimit = 1);
        });


        _bus.ConnectReceiveEndpoint("tenant3", e =>
        {
            var mqCfg = e as RabbitMqReceiveEndpointConfiguration;
            mqCfg.Bind<TenantMessage>(cb => { cb.RoutingKey = "tenant3"; });
            mqCfg.Consumer<TenantConsumer>(_serviceProvider, cfg => cfg.ConcurrentMessageLimit = 1);
        });


        for (var i = 0; i < 10; i++)
        {
            await _bus.Publish(new TenantMessage()
            {
                CorrelationId = Guid.NewGuid(),
                TenantId = "tenant1",
            }, cancellationToken);
        }

        for (var i = 0; i < 10; i++)
        {
            await Task.Delay(2000, cancellationToken);
            await _bus.Publish(new TenantMessage()
            {
                CorrelationId = Guid.NewGuid(),
                TenantId = "tenant2",
            }, cancellationToken);
        }

        for (var i = 0; i < 10; i++)
        {
            await Task.Delay(2000, cancellationToken);
            await _bus.Publish(new TenantMessage()
            {
                CorrelationId = Guid.NewGuid(),
                TenantId = "tenant3",
            }, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}