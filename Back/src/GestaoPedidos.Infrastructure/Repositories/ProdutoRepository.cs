using GestaoPedidos.Domain.Produtos;
using GestaoPedidos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestaoPedidos.Infrastructure.Repositories;

public class ProdutoRepository(GestaoPedidosDbContext context) : IProdutoRepository
{
    public Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Produtos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Produto>> ObterPorIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idsDistintos = ids.Distinct().ToList();
        return await context.Produtos.Where(p => idsDistintos.Contains(p.Id)).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken cancellationToken = default) =>
        await context.Produtos.AsNoTracking().OrderBy(p => p.Nome).ToListAsync(cancellationToken);

    public void Adicionar(Produto produto) => context.Produtos.Add(produto);
}
