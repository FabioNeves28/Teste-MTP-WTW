using FluentValidation;
using GestaoPedidos.Domain.Clientes;
using GestaoPedidos.Domain.Common;
using MediatR;

namespace GestaoPedidos.Application.Clientes;

public record CriarClienteCommand(string Nome, string Email, string Documento) : IRequest<ClienteDto>, IDadosCliente;

public class CriarClienteValidator : AbstractValidator<CriarClienteCommand>
{
    public CriarClienteValidator() => Include(new DadosClienteValidator());
}

public class CriarClienteHandler(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CriarClienteCommand, ClienteDto>
{
    public async Task<ClienteDto> Handle(CriarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = Cliente.Criar(request.Nome, request.Email, request.Documento);

        if (await clienteRepository.ExisteOutroComMesmoEmailOuDocumentoAsync(cliente, cancellationToken))
            throw new DomainException("Já existe um cliente com este e-mail ou documento.");

        clienteRepository.Adicionar(cliente);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ClienteDto.De(cliente);
    }
}
