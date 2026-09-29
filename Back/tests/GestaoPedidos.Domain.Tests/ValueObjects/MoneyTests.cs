using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.ValueObjects;

namespace GestaoPedidos.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Money_DeveArredondarParaDuasCasas()
    {
        new Money(10.005m).Valor.Should().Be(10.01m);
    }

    [Fact]
    public void Money_Negativo_DeveLancarExcecao()
    {
        var acao = () => new Money(-0.01m);

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Money_ComMesmoValor_DeveSerIgual()
    {
        new Money(10m).Should().Be(new Money(10.00m));
    }

    [Fact]
    public void Operadores_DevemSomarEMultiplicar()
    {
        var resultado = new Money(10m) * 3 + new Money(0.5m);

        resultado.Valor.Should().Be(30.5m);
    }
}
