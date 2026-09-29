using GestaoPedidos.Domain.Pedidos;
using GestaoPedidos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestaoPedidos.Infrastructure.Repositories;

public class PedidoRepository(GestaoPedidosDbContext context) : IPedidoRepository
{
    public Task<Pedido?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Pedidos.Include(p => p.Itens).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Pedido>> ListarAsync(StatusPedido? status = null, CancellationToken cancellationToken = default)
    {
        var query = context.Pedidos.AsNoTracking().Include(p => p.Itens).AsQueryable();

        if (status is not null)
            query = query.Where(p => p.Status == status);

        return await query.OrderByDescending(p => p.CriadoEm).ToListAsync(cancellationToken);
    }

    public void Adicionar(Pedido pedido) => context.Pedidos.Add(pedido);
}
