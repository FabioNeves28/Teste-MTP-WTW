using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.ValueObjects;

namespace GestaoPedidos.Domain.Produtos;

public class Produto : Entity
{
    public const int NomeTamanhoMaximo = 150;
    public const int DescricaoTamanhoMaximo = 1000;

    public string Nome { get; private set; } = null!;
    public string Descricao { get; private set; } = null!;
    public Money Preco { get; private set; } = null!;
    public int QuantidadeEstoque { get; private set; }

    private Produto() { }

    public static Produto Criar(string nome, string descricao, decimal preco, int quantidadeEstoque)
    {
        var produto = new Produto();
        produto.Atualizar(nome, descricao, preco, quantidadeEstoque);
        return produto;
    }

    public void Atualizar(string nome, string descricao, decimal preco, int quantidadeEstoque)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome do produto é obrigatório.");

        if (nome.Trim().Length > NomeTamanhoMaximo)
            throw new DomainException($"Nome do produto deve ter no máximo {NomeTamanhoMaximo} caracteres.");

        if (descricao?.Trim().Length > DescricaoTamanhoMaximo)
            throw new DomainException($"Descrição do produto deve ter no máximo {DescricaoTamanhoMaximo} caracteres.");

        if (quantidadeEstoque < 0)
            throw new DomainException("Quantidade em estoque não pode ser negativa.");

        Nome = nome.Trim();
        Descricao = descricao?.Trim() ?? string.Empty;
        Preco = new Money(preco);
        QuantidadeEstoque = quantidadeEstoque;
    }

    public bool PossuiEstoque(int quantidade) => QuantidadeEstoque >= quantidade;

    public void BaixarEstoque(int quantidade)
    {
        GarantirQuantidadePositiva(quantidade);

        if (!PossuiEstoque(quantidade))
            throw new DomainException(
                $"Estoque insuficiente para o produto '{Nome}'. Disponível: {QuantidadeEstoque}, solicitado: {quantidade}.");

        QuantidadeEstoque -= quantidade;
    }

    public void RestaurarEstoque(int quantidade)
    {
        GarantirQuantidadePositiva(quantidade);
        QuantidadeEstoque += quantidade;
    }

    private static void GarantirQuantidadePositiva(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("Quantidade deve ser maior que zero.");
    }
}
