using FluentValidation;
using Ordering.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        readonly IOrderRepository _orderRepository;

        public UpdateOrderCommandValidator(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;

            RuleFor(p => p.Id).Custom(async (id, context) => 
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    context.AddFailure($"order {id} is not found.");
                }
            });
            RuleFor(p => p.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{UserName} must not excced 50 characters.");

            RuleFor(p => p.EmailAddress)
                .NotEmpty().WithMessage("EmailAddress is required.");

            RuleFor(p => p.TotalPrice)
                .NotEmpty().WithMessage("{TotalPrice} is required.")
                .GreaterThan(0).WithMessage("{TotalPrice} should be greater than zero.");

        }
    }
}
