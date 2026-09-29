using FluentValidation;
using GestaoPedidos.Application.Common;
using GestaoPedidos.Domain.Clientes;
using GestaoPedidos.Domain.Common;
using MediatR;

namespace GestaoPedidos.Application.Clientes;

public record AtualizarClienteCommand(Guid Id, string Nome, string Email, string Documento)
    : IRequest<ClienteDto>, IDadosCliente;

public class AtualizarClienteValidator : AbstractValidator<AtualizarClienteCommand>
{
    public AtualizarClienteValidator() => Include(new DadosClienteValidator());
}

public class AtualizarClienteHandler(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<AtualizarClienteCommand, ClienteDto>
{
    public async Task<ClienteDto> Handle(AtualizarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.ObterPorIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Cliente", request.Id);

        cliente.Atualizar(request.Nome, request.Email, request.Documento);

        if (await clienteRepository.ExisteOutroComMesmoEmailOuDocumentoAsync(cliente, cancellationToken))
            throw new DomainException("Já existe um cliente com este e-mail ou documento.");

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ClienteDto.De(cliente);
    }
}
