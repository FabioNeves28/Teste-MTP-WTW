using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.Produtos;

namespace GestaoPedidos.Domain.Tests.Produtos;

public class ProdutoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DevePreencherPropriedades()
    {
        var produto = Produto.Criar("  Mouse  ", "Mouse sem fio", 89.9m, 5);

        produto.Nome.Should().Be("Mouse");
        produto.Preco.Valor.Should().Be(89.90m);
        produto.QuantidadeEstoque.Should().Be(5);
    }

    [Theory]
    [InlineData("", 10, 1)]
    [InlineData("Mouse", -1, 1)]
    [InlineData("Mouse", 10, -1)]
    public void Criar_ComDadosInvalidos_DeveLancarExcecao(string nome, decimal preco, int estoque)
    {
        var acao = () => Produto.Criar(nome, "descrição", preco, estoque);

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void BaixarEstoque_ComSaldo_DeveReduzirQuantidade()
    {
        var produto = Produto.Criar("Mouse", "", 10m, 5);

        produto.BaixarEstoque(3);

        produto.QuantidadeEstoque.Should().Be(2);
    }

    [Fact]
    public void BaixarEstoque_SemSaldo_DeveLancarExcecao()
    {
        var produto = Produto.Criar("Mouse", "", 10m, 2);

        var acao = () => produto.BaixarEstoque(3);

        acao.Should().Throw<DomainException>().WithMessage("Estoque insuficiente*");
        produto.QuantidadeEstoque.Should().Be(2);
    }

    [Fact]
    public void RestaurarEstoque_DeveAumentarQuantidade()
    {
        var produto = Produto.Criar("Mouse", "", 10m, 2);

        produto.RestaurarEstoque(3);

        produto.QuantidadeEstoque.Should().Be(5);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void MovimentarEstoque_ComQuantidadeInvalida_DeveLancarExcecao(int quantidade)
    {
        var produto = Produto.Criar("Mouse", "", 10m, 2);

        produto.Invoking(p => p.BaixarEstoque(quantidade)).Should().Throw<DomainException>();
        produto.Invoking(p => p.RestaurarEstoque(quantidade)).Should().Throw<DomainException>();
    }
}
