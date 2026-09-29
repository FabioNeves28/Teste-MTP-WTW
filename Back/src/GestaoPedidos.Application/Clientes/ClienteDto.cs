using GestaoPedidos.Domain.Clientes;

namespace GestaoPedidos.Application.Clientes;

public record ClienteDto(Guid Id, string Nome, string Email, string Documento)
{
    public static ClienteDto De(Cliente cliente) =>
        new(cliente.Id, cliente.Nome, cliente.Email.Endereco, cliente.Documento);
}
