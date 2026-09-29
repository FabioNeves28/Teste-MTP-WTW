using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.Pedidos;
using GestaoPedidos.Domain.Pedidos.Events;
using GestaoPedidos.Domain.Produtos;

namespace GestaoPedidos.Domain.Tests.Pedidos;

public class PedidoTests
{
    private static readonly Guid ClienteId = Guid.NewGuid();

    private static Produto NovoProduto(decimal preco = 10m, int estoque = 10) =>
        Produto.Criar("Teclado", "Teclado mecânico", preco, estoque);

    private static Pedido NovoPedido(params (Produto Produto, int Quantidade)[] itens) =>
        Pedido.Criar(ClienteId, itens);

    [Fact]
    public void Criar_ComItensValidos_DeveIniciarComStatusCriadoETotalCalculado()
    {
        var teclado = NovoProduto(preco: 150m);
        var mouse = NovoProduto(preco: 49.90m);

        var pedido = NovoPedido((teclado, 2), (mouse, 1));

        pedido.Status.Should().Be(StatusPedido.Criado);
        pedido.Itens.Should().HaveCount(2);
        pedido.Total.Valor.Should().Be(349.90m);
        pedido.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Criar_SemItens_DeveLancarExcecao()
    {
        var acao = () => NovoPedido();

        acao.Should().Throw<DomainException>().WithMessage("Pedido deve ter ao menos um item.");
    }

    [Fact]
    public void Criar_SemCliente_DeveLancarExcecao()
    {
        var acao = () => Pedido.Criar(Guid.Empty, [(NovoProduto(), 1)]);

        acao.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Criar_ComQuantidadeInvalida_DeveLancarExcecao(int quantidade)
    {
        var acao = () => NovoPedido((NovoProduto(), quantidade));

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Criar_ComQuantidadeMaiorQueEstoque_DeveLancarExcecao()
    {
        var acao = () => NovoPedido((NovoProduto(estoque: 3), 4));

        acao.Should().Throw<DomainException>().WithMessage("Estoque insuficiente*");
    }

    [Fact]
    public void Criar_ComMesmoProdutoRepetido_DeveConsolidarEmUmItem()
    {
        var produto = NovoProduto(preco: 10m, estoque: 10);

        var pedido = NovoPedido((produto, 2), (produto, 3));

        pedido.Itens.Should().ContainSingle().Which.Quantidade.Should().Be(5);
        pedido.Total.Valor.Should().Be(50m);
    }

    [Fact]
    public void Criar_ComProdutoRepetidoSomandoMaisQueEstoque_DeveLancarExcecao()
    {
        var produto = NovoProduto(estoque: 4);

        var acao = () => NovoPedido((produto, 2), (produto, 3));

        acao.Should().Throw<DomainException>().WithMessage("Estoque insuficiente*");
    }

    [Fact]
    public void Item_DeveManterPrecoDoMomentoDoPedido()
    {
        var produto = NovoProduto(preco: 100m);
        var pedido = NovoPedido((produto, 1));

        produto.Atualizar(produto.Nome, produto.Descricao, 200m, produto.QuantidadeEstoque);

        pedido.Itens.Single().PrecoUnitario.Valor.Should().Be(100m);
        pedido.Total.Valor.Should().Be(100m);
    }

    [Fact]
    public void AdicionarItem_EmPedidoCriado_DeveAtualizarTotal()
    {
        var pedido = NovoPedido((NovoProduto(preco: 10m), 1));

        pedido.AdicionarItem(NovoProduto(preco: 5m), 2);

        pedido.Itens.Should().HaveCount(2);
        pedido.Total.Valor.Should().Be(20m);
    }

    [Fact]
    public void AdicionarItem_EmPedidoConfirmado_DeveLancarExcecao()
    {
        var produto = NovoProduto();
        var pedido = NovoPedido((produto, 1));
        pedido.Confirmar([produto]);

        var acao = () => pedido.AdicionarItem(NovoProduto(), 1);

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void RemoverItem_UnicoItem_DeveLancarExcecao()
    {
        var produto = NovoProduto();
        var pedido = NovoPedido((produto, 1));

        var acao = () => pedido.RemoverItem(produto.Id);

        acao.Should().Throw<DomainException>().WithMessage("Pedido deve ter ao menos um item.");
    }

    [Fact]
    public void RemoverItem_ComMaisDeUmItem_DeveRemover()
    {
        var teclado = NovoProduto();
        var mouse = NovoProduto();
        var pedido = NovoPedido((teclado, 1), (mouse, 1));

        pedido.RemoverItem(mouse.Id);

        pedido.Itens.Should().ContainSingle(i => i.ProdutoId == teclado.Id);
    }

    [Fact]
    public void Confirmar_PedidoCriado_DeveMudarStatusEDispararEvento()
    {
        var produto = NovoProduto(estoque: 10);
        var pedido = NovoPedido((produto, 3));

        pedido.Confirmar([produto]);

        pedido.Status.Should().Be(StatusPedido.Confirmado);
        var evento = pedido.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<PedidoConfirmadoEvent>().Subject;
        evento.PedidoId.Should().Be(pedido.Id);
        evento.Itens.Should().ContainSingle().Which.Should().Be(new ItemMovimentado(produto.Id, 3));
    }

    [Fact]
    public void Confirmar_NaoDeveAlterarEstoqueDiretamente()
    {
        var produto = NovoProduto(estoque: 10);
        var pedido = NovoPedido((produto, 3));

        pedido.Confirmar([produto]);

        produto.QuantidadeEstoque.Should().Be(10);
    }

    [Fact]
    public void Confirmar_QuandoEstoqueDiminuiuAposCriacao_DeveLancarExcecao()
    {
        var produto = NovoProduto(estoque: 5);
        var pedido = NovoPedido((produto, 4));
        produto.BaixarEstoque(3);

        var acao = () => pedido.Confirmar([produto]);

        acao.Should().Throw<DomainException>().WithMessage("Estoque insuficiente*");
        pedido.Status.Should().Be(StatusPedido.Criado);
        pedido.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Confirmar_SemProdutoInformado_DeveLancarExcecao()
    {
        var pedido = NovoPedido((NovoProduto(), 1));

        var acao = () => pedido.Confirmar([]);

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancelar_PedidoCriado_NaoDeveDispararEvento()
    {
        var pedido = NovoPedido((NovoProduto(), 1));

        pedido.Cancelar();

        pedido.Status.Should().Be(StatusPedido.Cancelado);
        pedido.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Cancelar_PedidoConfirmado_DeveDispararEventoParaRestaurarEstoque()
    {
        var produto = NovoProduto();
        var pedido = NovoPedido((produto, 2));
        pedido.Confirmar([produto]);
        pedido.LimparEventos();

        pedido.Cancelar();

        pedido.Status.Should().Be(StatusPedido.Cancelado);
        var evento = pedido.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<PedidoCanceladoEvent>().Subject;
        evento.Itens.Should().ContainSingle().Which.Should().Be(new ItemMovimentado(produto.Id, 2));
    }

    [Fact]
    public void Finalizar_PedidoConfirmado_DeveMudarStatus()
    {
        var produto = NovoProduto();
        var pedido = NovoPedido((produto, 1));
        pedido.Confirmar([produto]);

        pedido.Finalizar();

        pedido.Status.Should().Be(StatusPedido.Finalizado);
    }

    [Fact]
    public void Finalizar_PedidoCriado_DeveLancarExcecao()
    {
        var pedido = NovoPedido((NovoProduto(), 1));

        var acao = () => pedido.Finalizar();

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void PedidoFinalizado_NaoPodeSerCancelado()
    {
        var pedido = PedidoFinalizado(out _);

        var acao = () => pedido.Cancelar();

        acao.Should().Throw<DomainException>().WithMessage("Pedido finalizado não pode ser alterado nem cancelado.");
    }

    [Fact]
    public void PedidoFinalizado_NaoPodeTerItensAlterados()
    {
        var pedido = PedidoFinalizado(out var produto);

        pedido.Invoking(p => p.AdicionarItem(NovoProduto(), 1)).Should().Throw<DomainException>();
        pedido.Invoking(p => p.RemoverItem(produto.Id)).Should().Throw<DomainException>();
    }

    [Fact]
    public void PedidoCancelado_NaoPodeSerConfirmadoNemCanceladoNovamente()
    {
        var produto = NovoProduto();
        var pedido = NovoPedido((produto, 1));
        pedido.Cancelar();

        pedido.Invoking(p => p.Confirmar([produto])).Should().Throw<DomainException>();
        pedido.Invoking(p => p.Cancelar()).Should().Throw<DomainException>();
        pedido.Invoking(p => p.Finalizar()).Should().Throw<DomainException>();
    }

    [Fact]
    public void PedidoConfirmado_NaoPodeSerConfirmadoNovamente()
    {
        var produto = NovoProduto();
        var pedido = NovoPedido((produto, 1));
        pedido.Confirmar([produto]);

        var acao = () => pedido.Confirmar([produto]);

        acao.Should().Throw<DomainException>();
    }

    private static Pedido PedidoFinalizado(out Produto produto)
    {
        produto = NovoProduto();
        var pedido = NovoPedido((produto, 1));
        pedido.Confirmar([produto]);
        pedido.Finalizar();
        return pedido;
    }
}
