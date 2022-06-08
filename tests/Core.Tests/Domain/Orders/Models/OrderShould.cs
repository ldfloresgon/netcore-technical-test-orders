using Core.Domain.Orders.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Core.Domain.Orders.Events;
using FluentAssertions;

namespace Core.Tests.Domain.Orders.Models
{
    public class OrderShould
    {
        [Fact]
        public void Create_OrderCreatedEvent_when_create_and_order()
        {
            const string PRODUCT_ID1 = "ProductId1";
            const string PRODUCT_ID2 = "ProductId2";
            const int QUANTITY_1 = 1;
            const int QUANTITY_2 = 2;

            var order = new Order(Guid.NewGuid());

            var products = new Dictionary<string, int>
            {
               { PRODUCT_ID1, QUANTITY_1 },
               { PRODUCT_ID2, QUANTITY_2 }
            };

            order.Create(products);

            var @event = (OrderCreatedEvent)order
                .DomainEvents
                .FirstOrDefault(d => d.GetType() == typeof(OrderCreatedEvent));

            @event.OrderId
                .Should()
                .Be(order.Id);

            @event.Products.Any(p => p.Id == PRODUCT_ID1 && p.Quantity == QUANTITY_1)
                .Should()
                .BeTrue();

            @event.Products.Any(p => p.Id == PRODUCT_ID2 && p.Quantity == QUANTITY_2)
                .Should()
                .BeTrue();
        }
    }
}
