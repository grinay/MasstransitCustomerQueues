using MassTransit;

namespace MasstransitCustomerQueues;

public class ExecuteCommand : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; }
    public string TenantId { get; set; }
}