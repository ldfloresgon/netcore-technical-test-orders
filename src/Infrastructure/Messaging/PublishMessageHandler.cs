using Core.Domain.Orders.Events;
using Infrastructure.Persistence;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Infrastructure.Tests")]
namespace Infrastructure.Messaging
{
    public class PublishMessageHandler : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        
        public PublishMessageHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        internal void DoWork(object state)
        {
            using (var scope= _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var events = context.Outbox
                   .Where(x => x.EventType == nameof(OrderCreatedEvent))
                   .ToList();

                foreach (var @event in events)
                {
                    var orderCreatedEvent = JsonConvert.DeserializeObject<OrderCreatedEvent>(@event.Data);
                    publishEndpoint.Publish(orderCreatedEvent);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
