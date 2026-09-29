using FluentValidation;
using GestaoPedidos.Application.Common;
using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.Produtos;
using MediatR;

namespace GestaoPedidos.Application.Produtos;

public record AtualizarProdutoCommand(Guid Id, string Nome, string Descricao, decimal Preco, int QuantidadeEstoque)
    : IRequest<ProdutoDto>, IDadosProduto;

public class AtualizarProdutoValidator : AbstractValidator<AtualizarProdutoCommand>
{
    public AtualizarProdutoValidator() => Include(new DadosProdutoValidator());
}

public class AtualizarProdutoHandler(IProdutoRepository produtoRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<AtualizarProdutoCommand, ProdutoDto>
{
    public async Task<ProdutoDto> Handle(AtualizarProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = await produtoRepository.ObterPorIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Produto", request.Id);

        produto.Atualizar(request.Nome, request.Descricao, request.Preco, request.QuantidadeEstoque);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ProdutoDto.De(produto);
    }
}
