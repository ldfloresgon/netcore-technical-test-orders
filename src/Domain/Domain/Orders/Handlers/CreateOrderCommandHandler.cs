using Core.Domain.Orders;
using Core.Domain.Orders.Models;
using Light.GuardClauses;
using MassTransit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Domain.Orders.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository.MustNotBeDefault();
        }

        public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order(request.Id);

            order.Create(request.Products);

            return await _orderRepository.Insert(order, cancellationToken);
        }
    }
}
