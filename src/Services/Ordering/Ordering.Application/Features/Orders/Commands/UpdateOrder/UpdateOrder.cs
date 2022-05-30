using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrder
    {
        public class Command : IRequest
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public decimal TotalPrice { get; set; }

            // BillingAddress
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
            public string AddressLine { get; set; }
            public string Country { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }

            // Payment
            public string CardName { get; set; }
            public string CardNumber { get; set; }
            public string Expiration { get; set; }
            public string CVV { get; set; }
            public int PaymentMethod { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.UserName)
                    .NotEmpty().WithMessage("{UserName} is required.")
                    .NotNull()
                    .MaximumLength(50).WithMessage("{UserName} must not exceed 50 characters");

                RuleFor(x => x.EmailAddress)
                    .NotEmpty().WithMessage("{EmailAddress} is required");

                RuleFor(x => x.TotalPrice)
                    .NotEmpty().WithMessage("{TotalPrice} is required.")
                    .GreaterThan(0).WithMessage("{TotalPrice} should be greater than zero.");
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IMapper _mapper;
            private readonly ILogger<UpdateOrder> _logger;

            public Handler(IOrderRepository orderRepository, IMapper mapper, ILogger<Handler> logger)
            {
                _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var orderToUpdate = await _orderRepository.GetByIdAsync(request.Id);
                if (orderToUpdate == null)
                {
                    _logger.LogError("Order not exist on database");
                    //throw new NotFoundException(nameof(Order), request.Id);
                }

                _mapper.Map(request, orderToUpdate, typeof(Command), typeof(Order));

                await _orderRepository.UpdateAsync(orderToUpdate);

                _logger.LogInformation($"Order {orderToUpdate.Id} is successfully updated.");

                return Unit.Value;
            }
        }
    }
}
