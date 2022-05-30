using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrder
    {
        public class Command : IRequest<int>
        {
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

        public class Handler : IRequestHandler<Command, int>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IMapper _mapper;
            private readonly IEmailService _emailService;
            private readonly ILogger<CheckoutOrder> _logger;

            public Handler(IOrderRepository orderRepository, IMapper mapper, IEmailService emailService, ILogger<CheckoutOrder> logger)
            {
                _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                var orderEntity = _mapper.Map<Order>(request);
                var newOrder = await _orderRepository.AddAsync(orderEntity);

                _logger.LogInformation($"Order {newOrder.Id} is successfully created.");

                await SendMail(newOrder);

                return newOrder.Id;
            }

            private async Task SendMail(Order order)
            {
                var email = new Email() { To = "ezozkme@gmail.com", Body = $"Order was created.", Subject = "Order was created" };

                try
                {
                    await _emailService.SendEmail(email);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Order {order.Id} failed due to an error with the mail service: {ex.Message}");
                }
            }
        }
    }
}
