using GestaoPedidos.Domain.Pedidos;

namespace GestaoPedidos.Application.Pedidos;

public record PedidoDto(
    Guid Id,
    Guid ClienteId,
    string ClienteNome,
    string Status,
    DateTime CriadoEm,
    DateTime? AtualizadoEm,
    decimal Total,
    IReadOnlyList<ItemPedidoDto> Itens)
{
    public static PedidoDto De(Pedido pedido, string clienteNome) =>
        new(
            pedido.Id,
            pedido.ClienteId,
            clienteNome,
            pedido.Status.ToString(),
            pedido.CriadoEm,
            pedido.AtualizadoEm,
            pedido.Total.Valor,
            pedido.Itens.Select(ItemPedidoDto.De).ToList());
}

public record ItemPedidoDto(Guid ProdutoId, string NomeProduto, decimal PrecoUnitario, int Quantidade, decimal Subtotal)
{
    public static ItemPedidoDto De(ItemPedido item) =>
        new(item.ProdutoId, item.NomeProduto, item.PrecoUnitario.Valor, item.Quantidade, item.Subtotal.Valor);
}
