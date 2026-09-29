using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.ValueObjects;

namespace GestaoPedidos.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Email_DeveSerNormalizado()
    {
        new Email("  Joao@Loja.COM ").Endereco.Should().Be("joao@loja.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("joao")]
    [InlineData("joao@loja")]
    [InlineData("joao @loja.com")]
    public void Email_Invalido_DeveLancarExcecao(string endereco)
    {
        var acao = () => new Email(endereco);

        acao.Should().Throw<DomainException>();
    }
}
