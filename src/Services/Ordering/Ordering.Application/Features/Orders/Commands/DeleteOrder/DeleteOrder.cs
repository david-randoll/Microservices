using Ordering.Application.Exceptions;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrder
    {
        public class Command : IRequest
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IMapper _mapper;
            private readonly ILogger<DeleteOrder> _logger;

            public Handler(IOrderRepository orderRepository, IMapper mapper, ILogger<DeleteOrder> logger)
            {
                _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var orderToDelete = await _orderRepository.GetByIdAsync(request.Id);
                if (orderToDelete == null)
                {
                    throw new NotFoundException(nameof(Order), request.Id);
                }

                await _orderRepository.DeleteAsync(orderToDelete);
                _logger.LogInformation($"Order {orderToDelete.Id} is successfully deleted.");

                return Unit.Value;
            }
        }
    }
}
