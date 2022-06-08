using Core.Domain.Orders.Events;
using Core.Domain.Orders.Models;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Infrastructure.Tests.Persistence.Repositories.Tests
{
    public class OrderRepositoryShould
    {
        public OrderRepositoryShould()
        {
            var contextMock = new Mock<DbContext>();
            var orders = new Mock<DbSet<Infrastructure.Persistence.Models.Order>>();
            var outbox = new Mock<DbSet<Infrastructure.Persistence.Models.Outbox>>();
            var database = new Mock<DatabaseFacade>(contextMock.Object);

            database
                .Setup(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transaction.Object);

            _dataBaseContext = new Mock<DataBaseContext>();

            _dataBaseContext.Setup(m => m.Orders).Returns(orders.Object);
            _dataBaseContext.Setup(m => m.Outbox).Returns(outbox.Object);
            _dataBaseContext.Setup(m => m.Database).Returns(database.Object);

        }

        [Fact]
        public async Task Persist_an_order()
        {
            Order order = CreateOrder();
            var orderRepository = new OrderRepository(_dataBaseContext.Object);

            var result = await orderRepository.Insert(order, default);

            var data = JsonConvert.SerializeObject(order.Products);

            Func<Infrastructure.Persistence.Models.Order, bool> validate =
                o => o.Id == order.Id && o.Data == data;

            result.Should().BeTrue(); 
            
            _dataBaseContext.Verify(_ => _.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _dataBaseContext.Verify(_ => _.Orders.AddAsync(It.Is<Infrastructure.Persistence.Models.Order>(m => validate(m)), It.IsAny<CancellationToken>()), Times.Once);
            _dataBaseContext.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Rollback_when_there_are_errors()
        {
            _dataBaseContext
                .Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            Order order = CreateOrder();
            OrderRepository orderRepository = new OrderRepository(_dataBaseContext.Object);

            var result = await orderRepository.Insert(order, default);

            result.Should().BeFalse();
            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Persist_events_in_outbox_table()
        {
            _dataBaseContext
                .Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            Order order = CreateOrder();
            OrderRepository orderRepository = new OrderRepository(_dataBaseContext.Object);

            await orderRepository.Insert(order, default);

            var @event = order
                .DomainEvents
                .FirstOrDefault(_ => _.GetType() == typeof(OrderCreatedEvent));

            var data = JsonConvert.SerializeObject(@event);

            Func<Infrastructure.Persistence.Models.Outbox, bool> validate =
                (outbox) => outbox.EventType == nameof(OrderCreatedEvent)
                && outbox.Data == data;

            _dataBaseContext.Verify(_ => _.Outbox.AddAsync(It.Is<Infrastructure.Persistence.Models.Outbox>(m => validate(m)), It.IsAny<CancellationToken>()), Times.Once);
        }

        private Order CreateOrder()
        {
            const string PRODUCT_ID1 = "ProductId1";
            const string PRODUCT_ID2 = "ProductId2";
            const int QUANTITY_1 = 1;
            const int QUANTITY_2 = 2;

            Order order = new Order(Guid.NewGuid());
            var products = new Dictionary<string, int>
            {
               { PRODUCT_ID1, QUANTITY_1 },
               { PRODUCT_ID2, QUANTITY_2 }
            };

            order.Create(products);

            return order;
        }

        Mock<DataBaseContext> _dataBaseContext = new Mock<DataBaseContext>();
        Mock<IDbContextTransaction> _transaction = new Mock<IDbContextTransaction>();
    }
}
