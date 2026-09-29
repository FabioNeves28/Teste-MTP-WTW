using GestaoPedidos.Application.Common;
using GestaoPedidos.Application.Pedidos.EventHandlers;
using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.Pedidos.Events;
using GestaoPedidos.Domain.Produtos;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace GestaoPedidos.Application.Tests.Pedidos;

public class EstoqueEventHandlersTests
{
    private readonly IProdutoRepository _produtoRepository = Substitute.For<IProdutoRepository>();
    private readonly Produto _teclado = Produto.Criar("Teclado", "", 100m, 10);
    private readonly Produto _mouse = Produto.Criar("Mouse", "", 50m, 5);

    public EstoqueEventHandlersTests()
    {
        _produtoRepository.ObterPorIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns([_teclado, _mouse]);
    }

    private BaixarEstoqueAoConfirmarPedidoHandler BaixarEstoqueHandler() =>
        new(_produtoRepository, NullLogger<BaixarEstoqueAoConfirmarPedidoHandler>.Instance);

    private RestaurarEstoqueAoCancelarPedidoHandler RestaurarEstoqueHandler() =>
        new(_produtoRepository, NullLogger<RestaurarEstoqueAoCancelarPedidoHandler>.Instance);

    private static DomainEventNotification<PedidoConfirmadoEvent> Confirmado(params ItemMovimentado[] itens) =>
        new(new PedidoConfirmadoEvent(Guid.NewGuid(), itens));

    private static DomainEventNotification<PedidoCanceladoEvent> Cancelado(params ItemMovimentado[] itens) =>
        new(new PedidoCanceladoEvent(Guid.NewGuid(), itens));

    [Fact]
    public async Task PedidoConfirmado_DeveBaixarEstoqueDeCadaProduto()
    {
        await BaixarEstoqueHandler().Handle(
            Confirmado(new(_teclado.Id, 3), new(_mouse.Id, 5)), CancellationToken.None);

        _teclado.QuantidadeEstoque.Should().Be(7);
        _mouse.QuantidadeEstoque.Should().Be(0);
    }

    [Fact]
    public async Task PedidoConfirmado_SemEstoqueSuficiente_DeveLancarExcecao()
    {
        var acao = () => BaixarEstoqueHandler().Handle(Confirmado(new ItemMovimentado(_mouse.Id, 6)), CancellationToken.None);

        await acao.Should().ThrowAsync<DomainException>().WithMessage("Estoque insuficiente*");
    }

    [Fact]
    public async Task PedidoConfirmado_ComProdutoInexistente_DeveLancarNotFound()
    {
        var acao = () => BaixarEstoqueHandler().Handle(Confirmado(new ItemMovimentado(Guid.NewGuid(), 1)), CancellationToken.None);

        await acao.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task PedidoCancelado_DeveRestaurarEstoqueDeCadaProduto()
    {
        await RestaurarEstoqueHandler().Handle(
            Cancelado(new(_teclado.Id, 2), new(_mouse.Id, 1)), CancellationToken.None);

        _teclado.QuantidadeEstoque.Should().Be(12);
        _mouse.QuantidadeEstoque.Should().Be(6);
    }
}
