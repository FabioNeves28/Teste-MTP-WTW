using FluentValidation;
using MediatR;

namespace GestaoPedidos.Application.Common;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var contexto = new ValidationContext<TRequest>(request);
        var resultados = await Task.WhenAll(validators.Select(v => v.ValidateAsync(contexto, cancellationToken)));
        var falhas = resultados.SelectMany(r => r.Errors).ToList();

        if (falhas.Count > 0)
            throw new ValidationException(falhas);

        return await next(cancellationToken);
    }
}
