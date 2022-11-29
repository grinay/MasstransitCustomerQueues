using MassTransit;

namespace MasstransitCustomerQueues;

public class CommandResult : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; }
    public string TenantId { get; set; }
}