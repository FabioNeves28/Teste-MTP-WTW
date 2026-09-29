using GestaoPedidos.Application.Common;
using GestaoPedidos.Application.Pedidos;
using GestaoPedidos.Domain.Clientes;
using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.Pedidos;
using GestaoPedidos.Domain.Pedidos.Events;
using GestaoPedidos.Domain.Produtos;
using NSubstitute;

namespace GestaoPedidos.Application.Tests.Pedidos;

public class AlterarStatusPedidoHandlerTests
{
    private readonly IPedidoRepository _pedidoRepository = Substitute.For<IPedidoRepository>();
    private readonly IClienteRepository _clienteRepository = Substitute.For<IClienteRepository>();
    private readonly IProdutoRepository _produtoRepository = Substitute.For<IProdutoRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly Produto _produto = Produto.Criar("Teclado", "", 100m, 10);
    private readonly Pedido _pedido;

    public AlterarStatusPedidoHandlerTests()
    {
        _pedido = Pedido.Criar(Guid.NewGuid(), [(_produto, 2)]);

        _pedidoRepository.ObterPorIdAsync(_pedido.Id, Arg.Any<CancellationToken>()).Returns(_pedido);
        _produtoRepository.ObterPorIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns([_produto]);
    }

    private AlterarStatusPedidoHandler Handler() =>
        new(_pedidoRepository, _clienteRepository, _produtoRepository, _unitOfWork);

    [Fact]
    public async Task Confirmar_DeveAlterarStatusGerarEventoESalvar()
    {
        var resultado = await Handler().Handle(new ConfirmarPedidoCommand(_pedido.Id), CancellationToken.None);

        resultado.Status.Should().Be("Confirmado");
        _pedido.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<PedidoConfirmadoEvent>();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Cancelar_PedidoCriado_NaoDeveGerarEvento()
    {
        var resultado = await Handler().Handle(new CancelarPedidoCommand(_pedido.Id), CancellationToken.None);

        resultado.Status.Should().Be("Cancelado");
        _pedido.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public async Task Finalizar_PedidoCriado_DeveLancarExcecaoESemSalvar()
    {
        var acao = () => Handler().Handle(new FinalizarPedidoCommand(_pedido.Id), CancellationToken.None);

        await acao.Should().ThrowAsync<DomainException>();
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PedidoInexistente_DeveLancarNotFound()
    {
        var acao = () => Handler().Handle(new ConfirmarPedidoCommand(Guid.NewGuid()), CancellationToken.None);

        await acao.Should().ThrowAsync<NotFoundException>();
    }
}
