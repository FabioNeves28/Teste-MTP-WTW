using GestaoPedidos.Domain.Produtos;

namespace GestaoPedidos.Application.Produtos;

public record ProdutoDto(Guid Id, string Nome, string Descricao, decimal Preco, int QuantidadeEstoque)
{
    public static ProdutoDto De(Produto produto) =>
        new(produto.Id, produto.Nome, produto.Descricao, produto.Preco.Valor, produto.QuantidadeEstoque);
}
