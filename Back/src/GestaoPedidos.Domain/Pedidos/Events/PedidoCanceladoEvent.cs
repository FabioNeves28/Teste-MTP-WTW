using GestaoPedidos.Domain.Common;

namespace GestaoPedidos.Domain.Pedidos.Events;

public sealed record PedidoCanceladoEvent(Guid PedidoId, IReadOnlyCollection<ItemMovimentado> Itens) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}
