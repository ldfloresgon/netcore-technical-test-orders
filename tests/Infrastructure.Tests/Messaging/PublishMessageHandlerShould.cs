using Core.Domain.Orders.Events;
using Infrastructure.Messaging;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Infrastructure.Tests.Messaging
{
    public class PublishMessageHandlerShould
    {
        public PublishMessageHandlerShould()
        {
            var contextMock = new Mock<DbContext>();
            var outbox = new Mock<DbSet<Infrastructure.Persistence.Models.Outbox>>();

            _dataBaseContext = new Mock<DataBaseContext>();

            _dataBaseContext.Setup(m => m.Outbox).Returns(outbox.Object);
        }

        [Fact]
        public void Retrieve_events_from_outbox_table_and_publish_to_message_broker()
        {
            Core.Domain.Orders.Models.Order order = CreateOrder();

            _dataBaseContext.Setup(_ => _.Outbox)
                .Returns(GetEvent(order));

            var publishEndpoint = new Mock<IPublishEndpoint>();

            var serviceProvider = MoqServiceProvider(publishEndpoint);

            var publishMessageHandler = new PublishMessageHandler(serviceProvider.Object);
            publishMessageHandler.DoWork(null);

            publishEndpoint.Verify(_ => _.Publish(It.Is<OrderCreatedEvent>(o => o.OrderId == order.Id), default), Times.Once);
        }

        private Mock<IServiceProvider> MoqServiceProvider(Mock<IPublishEndpoint> publishEndpoint)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(DataBaseContext)))
                .Returns(_dataBaseContext.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IPublishEndpoint)))
                .Returns(publishEndpoint.Object);

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);

            return serviceProvider;
        }

        private DbSet<Outbox> GetEvent(Core.Domain.Orders.Models.Order order)
        {
            var outbox = new Mock<DbSet<Outbox>>();

            var @event = order
                .DomainEvents
                .FirstOrDefault(_ => _.GetType() == typeof(OrderCreatedEvent));

            var data = new List<Outbox>
            {
                new Outbox {
                    Id = Guid.NewGuid(),
                    Data = JsonConvert.SerializeObject(@event),
                    EventType = nameof(OrderCreatedEvent)
                }
            }.AsQueryable();

            outbox.As<IQueryable<Outbox>>().Setup(m => m.Provider).Returns(data.Provider);
            outbox.As<IQueryable<Outbox>>().Setup(m => m.Expression).Returns(data.Expression);
            outbox.As<IQueryable<Outbox>>().Setup(m => m.ElementType).Returns(data.ElementType);
            outbox.As<IQueryable<Outbox>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return outbox.Object;
        }

        private Core.Domain.Orders.Models.Order CreateOrder()
        {
            const string PRODUCT_ID1 = "ProductId1";
            const string PRODUCT_ID2 = "ProductId2";
            const int QUANTITY_1 = 1;
            const int QUANTITY_2 = 2;

            Core.Domain.Orders.Models.Order order = new Core.Domain.Orders.Models.Order(Guid.NewGuid());
            var products = new Dictionary<string, int>
            {
               { PRODUCT_ID1, QUANTITY_1 },
               { PRODUCT_ID2, QUANTITY_2 }
            };

            order.Create(products);

            return order;
        }
        Mock<DataBaseContext> _dataBaseContext = new Mock<DataBaseContext>();
    }
}
