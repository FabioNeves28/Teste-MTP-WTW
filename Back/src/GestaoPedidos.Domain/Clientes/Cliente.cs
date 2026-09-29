using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.ValueObjects;

namespace GestaoPedidos.Domain.Clientes;

public class Cliente : Entity
{
    public const int NomeTamanhoMaximo = 150;

    public string Nome { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string Documento { get; private set; } = null!;

    private Cliente() { }

    public static Cliente Criar(string nome, string email, string documento)
    {
        var cliente = new Cliente();
        cliente.Atualizar(nome, email, documento);
        return cliente;
    }

    public void Atualizar(string nome, string email, string documento)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome do cliente é obrigatório.");

        if (nome.Trim().Length > NomeTamanhoMaximo)
            throw new DomainException($"Nome do cliente deve ter no máximo {NomeTamanhoMaximo} caracteres.");

        Nome = nome.Trim();
        Email = new Email(email);
        Documento = NormalizarDocumento(documento);
    }

    private static string NormalizarDocumento(string documento)
    {
        var numeros = new string((documento ?? string.Empty).Where(char.IsDigit).ToArray());

        if (numeros.Length is not (11 or 14))
            throw new DomainException("Documento deve ser um CPF (11 dígitos) ou CNPJ (14 dígitos).");

        return numeros;
    }
}
