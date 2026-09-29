using GestaoPedidos.Domain.Pedidos;
using GestaoPedidos.Domain.Produtos;
using GestaoPedidos.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoPedidos.Infrastructure.Persistence.Configurations;

public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.ToTable("ItensPedido");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();
        builder.Ignore(i => i.DomainEvents);
        builder.Ignore(i => i.Subtotal);

        builder.Property(i => i.NomeProduto).HasMaxLength(Produto.NomeTamanhoMaximo).IsRequired();
        builder.Property(i => i.PrecoUnitario)
            .HasConversion(preco => preco.Valor, valor => new Money(valor))
            .HasPrecision(18, 2);
        builder.Property(i => i.Quantidade);

        builder.HasOne<Produto>()
            .WithMany()
            .HasForeignKey(i => i.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new { i.PedidoId, i.ProdutoId }).IsUnique();
    }
}
