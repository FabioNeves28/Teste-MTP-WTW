using FluentValidation;
using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.Produtos;
using MediatR;

namespace GestaoPedidos.Application.Produtos;

public record CriarProdutoCommand(string Nome, string Descricao, decimal Preco, int QuantidadeEstoque)
    : IRequest<ProdutoDto>, IDadosProduto;

public class CriarProdutoValidator : AbstractValidator<CriarProdutoCommand>
{
    public CriarProdutoValidator() => Include(new DadosProdutoValidator());
}

public class CriarProdutoHandler(IProdutoRepository produtoRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CriarProdutoCommand, ProdutoDto>
{
    public async Task<ProdutoDto> Handle(CriarProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = Produto.Criar(request.Nome, request.Descricao, request.Preco, request.QuantidadeEstoque);

        produtoRepository.Adicionar(produto);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ProdutoDto.De(produto);
    }
}
