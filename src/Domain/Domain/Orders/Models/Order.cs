using System;
using System.Linq;
using System.Collections.Generic;
using Core.Domain.Orders.Events;

namespace Core.Domain.Orders.Models
{
    public class Order : AggregateRoot<Guid>
    {
        private List<Product> _products = new List<Product>();
        public IEnumerable<Product> Products => _products.ToArray();

        public Order(Guid id) : base(id)
        {            
        }

        public void Create(Dictionary<string, int> items)
        {
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = Id
            };

            var productAdded = new List<ProductAdded>();

            foreach (var item in items)
            {
                var product = new Product(item.Key, item.Value);

                _products.Add(product);

                productAdded.Add(new ProductAdded
                {
                    Id = product.Id,
                    Quantity = product.Quantity
                });
            }
            
            orderCreatedEvent.Products.AddRange(productAdded);

            AddEvent(orderCreatedEvent);
        }
    }
}
