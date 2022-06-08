using Core.Domain.Orders.Handlers;
using FluentAssertions;
using Infrastructure.WebApi.Controllers.Orders;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.WebApi.Tests
{
    public class OrdersControllerShould
    {
        public OrdersControllerShould()
        {
            _ordersController = new OrdersController(_mediator.Object);
            _ordersController.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task Return_status_code_BadRequest_when_the_operation_is_not_success()
        {
            _mediator
                .Setup(_ => _.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _ordersController.Create(new Request());

            result.Should().BeOfType(typeof(BadRequestResult));
        }

        [Fact]
        public async Task Return_status_code_Created_when_the_operation_is_success()
        {
            _mediator
                .Setup(_ => _.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _ordersController.Create(new Request()) as StatusCodeResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        OrdersController _ordersController;
        Mock<IMediator> _mediator = new Mock<IMediator>();
    }
}
