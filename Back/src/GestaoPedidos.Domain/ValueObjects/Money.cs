using GestaoPedidos.Domain.Common;

namespace GestaoPedidos.Domain.ValueObjects;

public sealed record Money
{
    public static readonly Money Zero = new(0m);

    public decimal Valor { get; }

    public Money(decimal valor)
    {
        if (valor < 0)
            throw new DomainException("Valor monetário não pode ser negativo.");

        Valor = decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
    }

    public static Money operator +(Money a, Money b) => new(a.Valor + b.Valor);

    public static Money operator *(Money a, int quantidade) => new(a.Valor * quantidade);

    public override string ToString() => Valor.ToString("F2");
}
