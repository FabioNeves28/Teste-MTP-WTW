using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.Produtos;
using GestaoPedidos.Domain.ValueObjects;

namespace GestaoPedidos.Domain.Pedidos;

public class ItemPedido : Entity
{
    public Guid PedidoId { get; private set; }
    public Guid ProdutoId { get; private set; }

    public string NomeProduto { get; private set; } = null!;
    public Money PrecoUnitario { get; private set; } = null!;
    public int Quantidade { get; private set; }

    public Money Subtotal => PrecoUnitario * Quantidade;

    private ItemPedido() { }

    internal ItemPedido(Guid pedidoId, Produto produto, int quantidade)
    {
        PedidoId = pedidoId;
        ProdutoId = produto.Id;
        NomeProduto = produto.Nome;
        PrecoUnitario = produto.Preco;
        Quantidade = quantidade;
    }

    internal void AumentarQuantidade(int quantidade) => Quantidade += quantidade;
}
