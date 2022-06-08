using System;

namespace Core.Messages
{
    public abstract class DomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Guid CorrelationId { get; set; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}