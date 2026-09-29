using FluentValidation;
using GestaoPedidos.Domain.Produtos;

namespace GestaoPedidos.Application.Produtos;

public interface IDadosProduto
{
    string Nome { get; }
    string Descricao { get; }
    decimal Preco { get; }
    int QuantidadeEstoque { get; }
}

public class DadosProdutoValidator : AbstractValidator<IDadosProduto>
{
    public DadosProdutoValidator()
    {
        RuleFor(p => p.Nome).NotEmpty().MaximumLength(Produto.NomeTamanhoMaximo);
        RuleFor(p => p.Descricao).MaximumLength(Produto.DescricaoTamanhoMaximo);
        RuleFor(p => p.Preco).GreaterThanOrEqualTo(0);
        RuleFor(p => p.QuantidadeEstoque).GreaterThanOrEqualTo(0);
    }
}
