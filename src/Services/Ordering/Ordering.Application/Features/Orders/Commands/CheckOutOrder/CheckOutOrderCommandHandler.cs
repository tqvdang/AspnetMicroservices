using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.CheckOutOrder
{
    public  class CheckOutOrderCommandHandler : IRequestHandler<CheckOutOrderCommand, int>
    {
        readonly IOrderRepository _orderRepository;
        readonly IMapper _mapper;
        readonly IEmailService _emailService;
        readonly ILogger<CheckOutOrderCommandHandler> _logger;
        public CheckOutOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IEmailService emailService, ILogger<CheckOutOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _emailService = emailService;   
            _logger = logger;
        }

        public async Task<int> Handle(CheckOutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = _mapper.Map<Order>(request);
            var newOrder = await _orderRepository.AddAsync(orderEntity);
            await SendMail(newOrder);
            return newOrder.Id;
        }
        private async Task SendMail(Order order) 
        {
            var email = new Email() { To="dang@test.com", Body = $"order {order.Id} was created", Subject="test" };
            try
            {
                await _emailService.SendEmail(email);
            
            }
            catch (Exception ex) 
            {
                _logger.LogError($"order {order.Id} failed. {ex}");
            }

        }
    }
}
