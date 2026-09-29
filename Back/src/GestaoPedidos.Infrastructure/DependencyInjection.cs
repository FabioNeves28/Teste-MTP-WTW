using GestaoPedidos.Domain.Clientes;
using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.Pedidos;
using GestaoPedidos.Domain.Produtos;
using GestaoPedidos.Infrastructure.Persistence;
using GestaoPedidos.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoPedidos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GestaoPedidosDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("GestaoPedidos")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<GestaoPedidosDbContext>());
        services.AddScoped<DomainEventDispatcher>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();

        return services;
    }
}
