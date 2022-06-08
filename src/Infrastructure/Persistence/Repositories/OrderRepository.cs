using System;
using System.Data;
using Dapper;
using System.Threading.Tasks;
using Core.Domain.Orders;
using Core.Domain.Orders.Models;
using Newtonsoft.Json;
using System.Threading;

namespace Infrastructure.Persistence.Repositories
{
    // This should not exist
    public class OrderRepository : IOrderRepository
    {
        private readonly DataBaseContext _dataBaseContext;

        public OrderRepository(DataBaseContext dataBaseContext)
        {
            _dataBaseContext = dataBaseContext;
        }


        public async Task<bool> Insert(Core.Domain.Orders.Models.Order order, CancellationToken cancellationToken)
        {
            using var transaction = await _dataBaseContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await _dataBaseContext.Orders.AddAsync(new Models.Order
                {
                    Id = order.Id,
                    Data = JsonConvert.SerializeObject(order.Products)
                }, cancellationToken);

                var domainEvents = order.DomainEvents;

                foreach (var domainEvent in domainEvents)
                {
                    await _dataBaseContext.Outbox.AddAsync(new Models.Outbox
                    {
                        Id = Guid.NewGuid(),
                        Created = DateTime.UtcNow,
                        EventType = domainEvent.GetType().Name,
                        Data = JsonConvert.SerializeObject(domainEvent)
                    }, cancellationToken);
                }

                await _dataBaseContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return true;

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }
        }
    }
}
