using MassTransit;

namespace MasstransitCustomerQueues;

public class TenantMessage : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; }
    public string TenantId { get; set; }
    public bool Aggregated { get; set; }
}