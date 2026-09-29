using GestaoPedidos.Domain.Clientes;
using GestaoPedidos.Domain.Common;

namespace GestaoPedidos.Domain.Tests.Clientes;

public class ClienteTests
{
    [Fact]
    public void Criar_ComDocumentoFormatado_DeveGuardarApenasNumeros()
    {
        var cliente = Cliente.Criar("Maria Silva", "Maria@Email.com", "123.456.789-09");

        cliente.Documento.Should().Be("12345678909");
        cliente.Email.Endereco.Should().Be("maria@email.com");
    }

    [Theory]
    [InlineData("", "maria@email.com", "12345678909")]
    [InlineData("Maria", "email-invalido", "12345678909")]
    [InlineData("Maria", "maria@email.com", "123")]
    public void Criar_ComDadosInvalidos_DeveLancarExcecao(string nome, string email, string documento)
    {
        var acao = () => Cliente.Criar(nome, email, documento);

        acao.Should().Throw<DomainException>();
    }
}
