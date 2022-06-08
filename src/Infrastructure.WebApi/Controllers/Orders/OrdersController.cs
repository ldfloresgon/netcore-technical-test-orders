using Core.Domain.Orders.Handlers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Infrastructure.WebApi.Controllers.Orders
{
    [ApiController]
    [Route("order")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Request request)
        {
            var command = new CreateOrderCommand
            {
                Id = request.Id,
                Products = request.Products
            };

            var success = await _mediator.Send(command, HttpContext.RequestAborted);

            if (success)
            {
                return StatusCode((int)HttpStatusCode.Created);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
