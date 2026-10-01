using System.Text.Json;
using FluentValidation;
using GestaoPedidos.Application.Common;
using GestaoPedidos.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoPedidos.Api;

public class ExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<ExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problem = exception switch
        {
            ValidationException validacao => new ValidationProblemDetails(
                validacao.Errors
                    .GroupBy(e => JsonNamingPolicy.CamelCase.ConvertName(e.PropertyName))
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Um ou mais campos são inválidos."
            },
            NotFoundException => Problem(StatusCodes.Status404NotFound, "Recurso não encontrado.", exception.Message),
            DomainException => Problem(StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada.", exception.Message),
            DbUpdateConcurrencyException => Problem(StatusCodes.Status409Conflict, "Conflito de concorrência.",
                "Os dados foram alterados por outra operação. Tente novamente."),
            _ => Problem(StatusCodes.Status500InternalServerError, "Erro interno.", "Ocorreu um erro inesperado.")
        };

        if (problem.Status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Erro não tratado em {Path}", httpContext.Request.Path);

        httpContext.Response.StatusCode = problem.Status!.Value;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception
        });
    }

    private static ProblemDetails Problem(int status, string title, string detail) =>
        new() { Status = status, Title = title, Detail = detail };
}
