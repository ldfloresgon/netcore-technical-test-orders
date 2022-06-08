using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Orders.Handlers
{
    public class CreateOrderCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public Dictionary<string, int> Products { get; set; }
    }
}
