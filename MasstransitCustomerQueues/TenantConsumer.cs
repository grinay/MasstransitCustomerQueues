using MassTransit;

namespace MasstransitCustomerQueues;

public class TenantConsumer : IConsumer<TenantMessage>
{
    private Guid InstanceId = Guid.NewGuid();
    private readonly IRequestClient<ExecuteCommand> _client;

    public TenantConsumer(IRequestClient<ExecuteCommand> client)
    {
        _client = client;
    }

    public async Task Consume(ConsumeContext<TenantMessage> context)
    {
        await _client.GetResponse<CommandResult>(new ExecuteCommand()
        {
            TenantId = context.Message.TenantId,
            CorrelationId = context.Message.CorrelationId
        });
    }
}