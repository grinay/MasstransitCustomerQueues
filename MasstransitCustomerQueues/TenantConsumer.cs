using MassTransit;

namespace MasstransitCustomerQueues;

public class TenantConsumer : IConsumer<TenantMessage>
{
    private readonly IRequestClient<TenantMessage> _client;

    public TenantConsumer(IRequestClient<TenantMessage> client)
    {
        _client = client;
    }

    public async Task Consume(ConsumeContext<TenantMessage> context)
    {
        context.Message.Aggregated = true;
        await _client.GetResponse<CommandResult>(context.Message);
    }
}