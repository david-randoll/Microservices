namespace Ordering.Application.Features.Orders.Queries.GetOrdersList
{
    public class GetOrdersList
    {
        public class Query : IRequest<List<OrdersVm>>
        {
            public string UserName { get; set; }

            public Query(string userName)
            {
                UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            }
        }

        public class Handler : IRequestHandler<Query, List<OrdersVm>>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IMapper _mapper;

            public Handler(IOrderRepository orderRepository, IMapper mapper)
            {
                _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<List<OrdersVm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var orderList = await _orderRepository.GetOrdersByUserName(request.UserName);
                return _mapper.Map<List<OrdersVm>>(orderList);
            }
        }

    }
}
