using FluentValidation;
using GestaoPedidos.Application.Common;
using GestaoPedidos.Application.Pedidos;

namespace GestaoPedidos.Application.Tests.Common;

public class ValidationBehaviorTests
{
    private static readonly ValidationBehavior<CriarPedidoCommand, PedidoDto> Behavior =
        new([new CriarPedidoValidator()]);

    [Fact]
    public async Task Handle_ComRequestInvalido_DeveLancarValidationExceptionSemChamarHandler()
    {
        var handlerChamado = false;
        var command = new CriarPedidoCommand(Guid.Empty, [new(Guid.NewGuid(), 0)]);

        var acao = () => Behavior.Handle(command, _ =>
        {
            handlerChamado = true;
            return Task.FromResult<PedidoDto>(null!);
        }, CancellationToken.None);

        var excecao = await acao.Should().ThrowAsync<ValidationException>();
        excecao.Which.Errors.Select(e => e.PropertyName)
            .Should().BeEquivalentTo("ClienteId", "Itens[0].Quantidade");
        handlerChamado.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ComPedidoSemItens_DeveRetornarMensagemDeNegocio()
    {
        var command = new CriarPedidoCommand(Guid.NewGuid(), []);

        var acao = () => Behavior.Handle(command, _ => Task.FromResult<PedidoDto>(null!), CancellationToken.None);

        var excecao = await acao.Should().ThrowAsync<ValidationException>();
        excecao.Which.Errors.Should().ContainSingle(e => e.ErrorMessage == "Pedido deve ter ao menos um item.");
    }

    [Fact]
    public async Task Handle_ComRequestValido_DeveChamarHandler()
    {
        var command = new CriarPedidoCommand(Guid.NewGuid(), [new(Guid.NewGuid(), 1)]);

        var acao = () => Behavior.Handle(command, _ => Task.FromResult<PedidoDto>(null!), CancellationToken.None);

        await acao.Should().NotThrowAsync();
    }
}
