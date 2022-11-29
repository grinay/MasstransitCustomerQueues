using MassTransit;

namespace MasstransitCustomerQueues;

public class CommandExecutor : IConsumer<ExecuteCommand>
{
    public async Task Consume(ConsumeContext<ExecuteCommand> context)
    {
        Console.WriteLine($"Message received for tenant {context.Message.TenantId}");
        await Task.Delay(5000);
        
        await context.RespondAsync(new CommandResult
        {
            TenantId = context.Message.TenantId,
            CorrelationId = context.Message.CorrelationId
        });
    }
}