using FluentValidation;
using GestaoPedidos.Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoPedidos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
