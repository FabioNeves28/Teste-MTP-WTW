using GestaoPedidos.Application.Common;
using GestaoPedidos.Domain.Clientes;
using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.Pedidos;
using GestaoPedidos.Domain.Produtos;
using MediatR;

namespace GestaoPedidos.Application.Pedidos;

public record ConfirmarPedidoCommand(Guid Id) : IRequest<PedidoDto>;

public record CancelarPedidoCommand(Guid Id) : IRequest<PedidoDto>;

public record FinalizarPedidoCommand(Guid Id) : IRequest<PedidoDto>;

public class AlterarStatusPedidoHandler(
    IPedidoRepository pedidoRepository,
    IClienteRepository clienteRepository,
    IProdutoRepository produtoRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ConfirmarPedidoCommand, PedidoDto>,
      IRequestHandler<CancelarPedidoCommand, PedidoDto>,
      IRequestHandler<FinalizarPedidoCommand, PedidoDto>
{
    public async Task<PedidoDto> Handle(ConfirmarPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await ObterPedidoAsync(request.Id, cancellationToken);
        var produtos = await produtoRepository.ObterPorIdsAsync(pedido.Itens.Select(i => i.ProdutoId), cancellationToken);

        pedido.Confirmar(produtos);

        return await SalvarAsync(pedido, cancellationToken);
    }

    public async Task<PedidoDto> Handle(CancelarPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await ObterPedidoAsync(request.Id, cancellationToken);
        pedido.Cancelar();

        return await SalvarAsync(pedido, cancellationToken);
    }

    public async Task<PedidoDto> Handle(FinalizarPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await ObterPedidoAsync(request.Id, cancellationToken);
        pedido.Finalizar();

        return await SalvarAsync(pedido, cancellationToken);
    }

    private async Task<Pedido> ObterPedidoAsync(Guid id, CancellationToken cancellationToken) =>
        await pedidoRepository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Pedido", id);

    private async Task<PedidoDto> SalvarAsync(Pedido pedido, CancellationToken cancellationToken)
    {
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var cliente = await clienteRepository.ObterPorIdAsync(pedido.ClienteId, cancellationToken);
        return PedidoDto.De(pedido, cliente?.Nome ?? string.Empty);
    }
}
