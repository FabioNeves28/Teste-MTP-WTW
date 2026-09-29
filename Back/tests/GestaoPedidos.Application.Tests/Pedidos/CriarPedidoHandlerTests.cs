using GestaoPedidos.Application.Common;
using GestaoPedidos.Application.Pedidos;
using GestaoPedidos.Domain.Clientes;
using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.Pedidos;
using GestaoPedidos.Domain.Produtos;
using NSubstitute;

namespace GestaoPedidos.Application.Tests.Pedidos;

public class CriarPedidoHandlerTests
{
    private readonly IPedidoRepository _pedidoRepository = Substitute.For<IPedidoRepository>();
    private readonly IClienteRepository _clienteRepository = Substitute.For<IClienteRepository>();
    private readonly IProdutoRepository _produtoRepository = Substitute.For<IProdutoRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly Cliente _cliente = Cliente.Criar("Ana", "ana@email.com", "52998224725");
    private readonly Produto _teclado = Produto.Criar("Teclado", "", 100m, 10);

    public CriarPedidoHandlerTests()
    {
        _clienteRepository.ObterPorIdAsync(_cliente.Id, Arg.Any<CancellationToken>()).Returns(_cliente);
        _produtoRepository.ObterPorIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns([_teclado]);
    }

    private CriarPedidoHandler Handler() => new(_pedidoRepository, _clienteRepository, _produtoRepository, _unitOfWork);

    [Fact]
    public async Task Handle_ComDadosValidos_DeveSalvarPedidoERetornarTotal()
    {
        var command = new CriarPedidoCommand(_cliente.Id, [new(_teclado.Id, 2), new(_teclado.Id, 1)]);

        var resultado = await Handler().Handle(command, CancellationToken.None);

        resultado.ClienteNome.Should().Be("Ana");
        resultado.Status.Should().Be("Criado");
        resultado.Total.Should().Be(300m);
        resultado.Itens.Should().ContainSingle().Which.Quantidade.Should().Be(3);
        _pedidoRepository.Received(1).Adicionar(Arg.Any<Pedido>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComClienteInexistente_DeveLancarNotFound()
    {
        var command = new CriarPedidoCommand(Guid.NewGuid(), [new(_teclado.Id, 1)]);

        var acao = () => Handler().Handle(command, CancellationToken.None);

        await acao.Should().ThrowAsync<NotFoundException>().WithMessage("Cliente*");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComProdutoInexistente_DeveLancarNotFound()
    {
        var command = new CriarPedidoCommand(_cliente.Id, [new(Guid.NewGuid(), 1)]);

        var acao = () => Handler().Handle(command, CancellationToken.None);

        await acao.Should().ThrowAsync<NotFoundException>().WithMessage("Produto*");
    }

    [Fact]
    public async Task Handle_ComQuantidadeMaiorQueEstoque_DeveLancarExcecaoDeDominio()
    {
        var command = new CriarPedidoCommand(_cliente.Id, [new(_teclado.Id, 11)]);

        var acao = () => Handler().Handle(command, CancellationToken.None);

        await acao.Should().ThrowAsync<DomainException>();
        _pedidoRepository.DidNotReceive().Adicionar(Arg.Any<Pedido>());
    }
}
