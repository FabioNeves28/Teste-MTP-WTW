using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.Pedidos.Events;
using GestaoPedidos.Domain.Produtos;
using GestaoPedidos.Domain.ValueObjects;

namespace GestaoPedidos.Domain.Pedidos;

public class Pedido : Entity
{
    private static readonly Dictionary<StatusPedido, StatusPedido[]> TransicoesPermitidas = new()
    {
        [StatusPedido.Criado] = [StatusPedido.Confirmado, StatusPedido.Cancelado],
        [StatusPedido.Confirmado] = [StatusPedido.Finalizado, StatusPedido.Cancelado],
    };

    private readonly List<ItemPedido> _itens = [];

    public Guid ClienteId { get; private set; }
    public StatusPedido Status { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }

    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

    public Money Total => _itens.Aggregate(Money.Zero, (total, item) => total + item.Subtotal);

    private Pedido() { }

    public static Pedido Criar(Guid clienteId, IEnumerable<(Produto Produto, int Quantidade)> itens)
    {
        if (clienteId == Guid.Empty)
            throw new DomainException("Cliente do pedido é obrigatório.");

        var pedido = new Pedido
        {
            ClienteId = clienteId,
            Status = StatusPedido.Criado,
            CriadoEm = DateTime.UtcNow
        };

        foreach (var (produto, quantidade) in itens)
            pedido.AdicionarOuConsolidarItem(produto, quantidade);

        if (pedido._itens.Count == 0)
            throw new DomainException("Pedido deve ter ao menos um item.");

        return pedido;
    }

    public void AdicionarItem(Produto produto, int quantidade)
    {
        GarantirQueItensPodemSerAlterados();
        AdicionarOuConsolidarItem(produto, quantidade);
        MarcarAtualizacao();
    }

    private void AdicionarOuConsolidarItem(Produto produto, int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("Quantidade do item deve ser maior que zero.");

        var itemExistente = _itens.SingleOrDefault(i => i.ProdutoId == produto.Id);
        var quantidadeTotal = (itemExistente?.Quantidade ?? 0) + quantidade;

        GarantirEstoqueDisponivel(produto, quantidadeTotal);

        if (itemExistente is null)
            _itens.Add(new ItemPedido(Id, produto, quantidade));
        else
            itemExistente.AumentarQuantidade(quantidade);
    }

    public void RemoverItem(Guid produtoId)
    {
        GarantirQueItensPodemSerAlterados();

        var item = _itens.SingleOrDefault(i => i.ProdutoId == produtoId)
            ?? throw new DomainException("Item não encontrado no pedido.");

        if (_itens.Count == 1)
            throw new DomainException("Pedido deve ter ao menos um item.");

        _itens.Remove(item);
        MarcarAtualizacao();
    }

    public void Confirmar(IReadOnlyCollection<Produto> produtosAtuais)
    {
        GarantirTransicao(StatusPedido.Confirmado);

        foreach (var item in _itens)
        {
            var produto = produtosAtuais.SingleOrDefault(p => p.Id == item.ProdutoId)
                ?? throw new DomainException($"Produto '{item.NomeProduto}' não está mais disponível.");

            GarantirEstoqueDisponivel(produto, item.Quantidade);
        }

        AlterarStatus(StatusPedido.Confirmado);
        AdicionarEvento(new PedidoConfirmadoEvent(Id, ItensMovimentados()));
    }

    public void Cancelar()
    {
        GarantirTransicao(StatusPedido.Cancelado);

        var estavaConfirmado = Status == StatusPedido.Confirmado;
        AlterarStatus(StatusPedido.Cancelado);

        if (estavaConfirmado)
            AdicionarEvento(new PedidoCanceladoEvent(Id, ItensMovimentados()));
    }

    public void Finalizar()
    {
        GarantirTransicao(StatusPedido.Finalizado);
        AlterarStatus(StatusPedido.Finalizado);
    }

    private void GarantirTransicao(StatusPedido novoStatus)
    {
        if (Status == StatusPedido.Finalizado)
            throw new DomainException("Pedido finalizado não pode ser alterado nem cancelado.");

        var permitido = TransicoesPermitidas.TryGetValue(Status, out var destinos) && destinos.Contains(novoStatus);

        if (!permitido)
            throw new DomainException($"Não é possível alterar o pedido de '{Status}' para '{novoStatus}'.");
    }

    private void GarantirQueItensPodemSerAlterados()
    {
        if (Status == StatusPedido.Finalizado)
            throw new DomainException("Pedido finalizado não pode ser alterado nem cancelado.");

        if (Status != StatusPedido.Criado)
            throw new DomainException($"Itens não podem ser alterados em um pedido '{Status}'.");
    }

    private static void GarantirEstoqueDisponivel(Produto produto, int quantidade)
    {
        if (!produto.PossuiEstoque(quantidade))
            throw new DomainException(
                $"Estoque insuficiente para o produto '{produto.Nome}'. Disponível: {produto.QuantidadeEstoque}, solicitado: {quantidade}.");
    }

    private List<ItemMovimentado> ItensMovimentados() =>
        _itens.Select(i => new ItemMovimentado(i.ProdutoId, i.Quantidade)).ToList();

    private void AlterarStatus(StatusPedido novoStatus)
    {
        Status = novoStatus;
        MarcarAtualizacao();
    }

    private void MarcarAtualizacao() => AtualizadoEm = DateTime.UtcNow;
}
