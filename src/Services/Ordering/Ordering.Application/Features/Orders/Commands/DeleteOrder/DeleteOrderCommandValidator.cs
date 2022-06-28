using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    internal class DeleteOrderCommandValidator : AbstractValidator <DeleteOrderCommand>
    {
        public DeleteOrderCommandValidator()
        {
            RuleFor(o => o.OrderId).GreaterThan(0).NotNull().WithMessage("order-id is required.");
        }
    }
}
