using GestaoPedidos.Domain.Clientes;
using GestaoPedidos.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoPedidos.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Ignore(c => c.DomainEvents);

        builder.Property(c => c.Nome).HasMaxLength(Cliente.NomeTamanhoMaximo).IsRequired();
        builder.Property(c => c.Email)
            .HasConversion(email => email.Endereco, endereco => new Email(endereco))
            .HasMaxLength(Email.TamanhoMaximo)
            .IsRequired();
        builder.Property(c => c.Documento).HasMaxLength(14).IsUnicode(false).IsRequired();

        builder.HasIndex(c => c.Email).IsUnique();
        builder.HasIndex(c => c.Documento).IsUnique();
    }
}
