using MassTransit;

namespace MasstransitCustomerQueues;

public class TenantConsumer : IConsumer<TenantMessage>
{
    private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(2);

    public async Task Consume(ConsumeContext<TenantMessage> context)
    {
        await Semaphore.WaitAsync();
        try
        {
            Console.WriteLine($"Message received for tenant {context.Message.TenantId}");
            await Task.Delay(5000);
        }
        finally
        {
            Semaphore.Release();
        }
    }
}