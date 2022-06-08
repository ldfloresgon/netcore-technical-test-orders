using Core.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain
{
    public abstract class AggregateRoot<T> : Entity<T>
    {
        private List<DomainEvent> _domainEvents = new List<DomainEvent>();
        public IEnumerable<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected AggregateRoot(T id) : base(id)
        {
        }

        public void AddEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }
}
