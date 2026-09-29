using GestaoPedidos.Application.Common;
using GestaoPedidos.Domain.Clientes;
using MediatR;

namespace GestaoPedidos.Application.Clientes;

public record ListarClientesQuery : IRequest<IReadOnlyList<ClienteDto>>;

public record ObterClienteQuery(Guid Id) : IRequest<ClienteDto>;

public class ConsultarClientesHandler(IClienteRepository clienteRepository)
    : IRequestHandler<ListarClientesQuery, IReadOnlyList<ClienteDto>>,
      IRequestHandler<ObterClienteQuery, ClienteDto>
{
    public async Task<IReadOnlyList<ClienteDto>> Handle(ListarClientesQuery request, CancellationToken cancellationToken)
    {
        var clientes = await clienteRepository.ListarAsync(cancellationToken);
        return clientes.Select(ClienteDto.De).ToList();
    }

    public async Task<ClienteDto> Handle(ObterClienteQuery request, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.ObterPorIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Cliente", request.Id);

        return ClienteDto.De(cliente);
    }
}
