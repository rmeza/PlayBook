// COMMAND: Modifica estado
public record CreateOrderCommand(Guid CustomerId, decimal Amount) : IRequest<Guid>;

// QUERY: Solo lectura (DTO directo)
public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDetailsDto>;