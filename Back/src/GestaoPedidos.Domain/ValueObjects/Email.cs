using System.Text.RegularExpressions;
using GestaoPedidos.Domain.Common;

namespace GestaoPedidos.Domain.ValueObjects;

public sealed partial record Email
{
    public const int TamanhoMaximo = 254;

    public string Endereco { get; }

    public Email(string endereco)
    {
        var normalizado = endereco?.Trim().ToLowerInvariant() ?? string.Empty;

        if (normalizado.Length is 0 or > TamanhoMaximo || !FormatoValido().IsMatch(normalizado))
            throw new DomainException($"E-mail '{endereco}' é inválido.");

        Endereco = normalizado;
    }

    public override string ToString() => Endereco;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex FormatoValido();
}
