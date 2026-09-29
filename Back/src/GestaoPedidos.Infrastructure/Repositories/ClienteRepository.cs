using GestaoPedidos.Domain.Clientes;
using GestaoPedidos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestaoPedidos.Infrastructure.Repositories;

public class ClienteRepository(GestaoPedidosDbContext context) : IClienteRepository
{
    public Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Clientes.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Cliente>> ListarAsync(CancellationToken cancellationToken = default) =>
        await context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ToListAsync(cancellationToken);

    public Task<bool> ExisteOutroComMesmoEmailOuDocumentoAsync(Cliente cliente, CancellationToken cancellationToken = default) =>
        context.Clientes.AnyAsync(
            c => c.Id != cliente.Id && (c.Email == cliente.Email || c.Documento == cliente.Documento),
            cancellationToken);

    public void Adicionar(Cliente cliente) => context.Clientes.Add(cliente);
}
