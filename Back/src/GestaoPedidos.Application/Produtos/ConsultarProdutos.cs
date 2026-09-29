using GestaoPedidos.Application.Common;
using GestaoPedidos.Domain.Produtos;
using MediatR;

namespace GestaoPedidos.Application.Produtos;

public record ListarProdutosQuery : IRequest<IReadOnlyList<ProdutoDto>>;

public record ObterProdutoQuery(Guid Id) : IRequest<ProdutoDto>;

public class ConsultarProdutosHandler(IProdutoRepository produtoRepository)
    : IRequestHandler<ListarProdutosQuery, IReadOnlyList<ProdutoDto>>,
      IRequestHandler<ObterProdutoQuery, ProdutoDto>
{
    public async Task<IReadOnlyList<ProdutoDto>> Handle(ListarProdutosQuery request, CancellationToken cancellationToken)
    {
        var produtos = await produtoRepository.ListarAsync(cancellationToken);
        return produtos.Select(ProdutoDto.De).ToList();
    }

    public async Task<ProdutoDto> Handle(ObterProdutoQuery request, CancellationToken cancellationToken)
    {
        var produto = await produtoRepository.ObterPorIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Produto", request.Id);

        return ProdutoDto.De(produto);
    }
}
