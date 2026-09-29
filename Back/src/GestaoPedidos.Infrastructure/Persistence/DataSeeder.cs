using GestaoPedidos.Domain.Clientes;
using GestaoPedidos.Domain.Produtos;
using Microsoft.EntityFrameworkCore;

namespace GestaoPedidos.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task PopularAsync(GestaoPedidosDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Produtos.AnyAsync(cancellationToken))
            return;

        context.Produtos.AddRange(
            Produto.Criar("Teclado mecânico", "Teclado ABNT2 com switches marrons", 349.90m, 15),
            Produto.Criar("Mouse sem fio", "Mouse óptico 1600 DPI", 89.90m, 40),
            Produto.Criar("Monitor 24\"", "Monitor Full HD IPS 75Hz", 899.00m, 8),
            Produto.Criar("Headset", "Headset com microfone e cancelamento de ruído", 259.50m, 20),
            Produto.Criar("Webcam Full HD", "Webcam 1080p com microfone embutido", 199.99m, 0));

        context.Clientes.AddRange(
            Cliente.Criar("Ana Souza", "ana.souza@email.com", "529.982.247-25"),
            Cliente.Criar("Bruno Lima", "bruno.lima@email.com", "123.456.789-09"),
            Cliente.Criar("Tech Store LTDA", "compras@techstore.com.br", "11.222.333/0001-81"));

        await context.SaveChangesAsync(cancellationToken);
    }
}
