using GestaoPedidos.Domain.Clientes;
using GestaoPedidos.Domain.ValueObjects;
using GestaoPedidos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestaoPedidos.Infrastructure.Repositories;

public class ClienteRepository(GestaoPedidosDbContext context) : IClienteRepository
{
    public Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Clientes.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Cliente>> ListarAsync(CancellationToken cancellationToken = default) =>
        await context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ToListAsync(cancellationToken);

    public Task<bool> ExisteComEmailAsync(Email email, Guid? ignorarClienteId = null, CancellationToken cancellationToken = default) =>
        context.Clientes.AnyAsync(c => c.Email == email && c.Id != ignorarClienteId, cancellationToken);

    public void Adicionar(Cliente cliente) => context.Clientes.Add(cliente);
}
