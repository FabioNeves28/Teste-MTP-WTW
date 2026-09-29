using GestaoPedidos.Domain.Produtos;
using GestaoPedidos.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoPedidos.Infrastructure.Persistence.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Ignore(p => p.DomainEvents);

        builder.Property(p => p.Nome).HasMaxLength(Produto.NomeTamanhoMaximo).IsRequired();
        builder.Property(p => p.Descricao).HasMaxLength(Produto.DescricaoTamanhoMaximo).IsRequired();
        builder.Property(p => p.Preco)
            .HasConversion(preco => preco.Valor, valor => new Money(valor))
            .HasPrecision(18, 2);
        builder.Property(p => p.QuantidadeEstoque);
        builder.Property<byte[]>("RowVersion").IsRowVersion();
    }
}
