using FluentValidation;
using GestaoPedidos.Domain.Clientes;
using GestaoPedidos.Domain.ValueObjects;

namespace GestaoPedidos.Application.Clientes;

public interface IDadosCliente
{
    string Nome { get; }
    string Email { get; }
    string Documento { get; }
}

public class DadosClienteValidator : AbstractValidator<IDadosCliente>
{
    public DadosClienteValidator()
    {
        RuleFor(c => c.Nome).NotEmpty().MaximumLength(Cliente.NomeTamanhoMaximo);
        RuleFor(c => c.Email).NotEmpty().EmailAddress().MaximumLength(Email.TamanhoMaximo);
        RuleFor(c => c.Documento).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(d => d.Count(char.IsDigit) is 11 or 14)
            .WithMessage("Documento deve ser um CPF (11 dígitos) ou CNPJ (14 dígitos).");
    }
}
