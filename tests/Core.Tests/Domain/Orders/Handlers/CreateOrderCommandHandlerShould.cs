using Core.Domain.Orders;
using Core.Domain.Orders.Handlers;
using Core.Domain.Orders.Models;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Domain.Orders.Handlers
{

    public class CreateOrderCommandHandlerShould
    {
        [Fact]
        public async Task Create_and_persist_and_order()
        {
            const string PRODUCT_ID1 = "ProductId1";
            const string PRODUCT_ID2 = "ProductId2";
            const int QUANTITY_1 = 1;
            const int QUANTITY_2 = 2;

            var orderRepository = new Mock<IOrderRepository>();

            var createOrderCommandHandler = new CreateOrderCommandHandler(orderRepository.Object);

            var command = new CreateOrderCommand
            {
                Id = Guid.NewGuid(),
                Products = new Dictionary<string, int>
                {
                    { PRODUCT_ID1, QUANTITY_1 },
                    { PRODUCT_ID2, QUANTITY_2 }
                }
            };

            var result = await createOrderCommandHandler.Handle(command, default);

            Func<Order, bool> validate = (order) => order.Id == command.Id
                && order.Products.Any(p => p.Id == PRODUCT_ID1)
                && order.Products.FirstOrDefault(p => p.Id == PRODUCT_ID1).Quantity == QUANTITY_1
                && order.Products.Any(p => p.Id == PRODUCT_ID2)
                && order.Products.FirstOrDefault(p => p.Id == PRODUCT_ID2).Quantity == QUANTITY_2
                ;

            orderRepository.Verify(_ => _.Insert(It.Is<Order>(order => validate(order)), default), Times.Once);
        }
    }
}
