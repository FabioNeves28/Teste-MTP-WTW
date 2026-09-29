using GestaoPedidos.Application.Common;
using GestaoPedidos.Domain.Clientes;
using GestaoPedidos.Domain.Pedidos;
using MediatR;

namespace GestaoPedidos.Application.Pedidos;

public record ListarPedidosQuery(StatusPedido? Status = null) : IRequest<IReadOnlyList<PedidoDto>>;

public record ObterPedidoQuery(Guid Id) : IRequest<PedidoDto>;

public class ConsultarPedidosHandler(IPedidoRepository pedidoRepository, IClienteRepository clienteRepository)
    : IRequestHandler<ListarPedidosQuery, IReadOnlyList<PedidoDto>>,
      IRequestHandler<ObterPedidoQuery, PedidoDto>
{
    public async Task<IReadOnlyList<PedidoDto>> Handle(ListarPedidosQuery request, CancellationToken cancellationToken)
    {
        var pedidos = await pedidoRepository.ListarAsync(request.Status, cancellationToken);
        var clientes = await clienteRepository.ListarAsync(cancellationToken);
        var nomes = clientes.ToDictionary(c => c.Id, c => c.Nome);

        return pedidos.Select(p => PedidoDto.De(p, nomes.GetValueOrDefault(p.ClienteId, string.Empty))).ToList();
    }

    public async Task<PedidoDto> Handle(ObterPedidoQuery request, CancellationToken cancellationToken)
    {
        var pedido = await pedidoRepository.ObterPorIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Pedido", request.Id);

        var cliente = await clienteRepository.ObterPorIdAsync(pedido.ClienteId, cancellationToken);
        return PedidoDto.De(pedido, cliente?.Nome ?? string.Empty);
    }
}
