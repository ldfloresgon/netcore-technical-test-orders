using Core.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Orders.Events
{
    public class OrderCreatedEvent : DomainEvent
    {
        public Guid OrderId { get; set; }
        public List<ProductAdded> Products { get; set; } = new List<ProductAdded>();
    }


    public class ProductAdded
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
    }
}
