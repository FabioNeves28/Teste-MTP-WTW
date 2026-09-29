using GestaoPedidos.Application.Common;
using GestaoPedidos.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GestaoPedidos.Infrastructure.Persistence;

public class DomainEventDispatcher(IPublisher publisher)
{
    public async Task DespacharAsync(DbContext context, CancellationToken cancellationToken)
    {
        while (ColetarEventos(context) is { Count: > 0 } eventos)
        {
            foreach (var evento in eventos)
                await publisher.Publish(CriarNotificacao(evento), cancellationToken);
        }
    }

    private static List<IDomainEvent> ColetarEventos(DbContext context)
    {
        var entidades = context.ChangeTracker.Entries<Entity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        var eventos = entidades.SelectMany(e => e.DomainEvents).ToList();
        entidades.ForEach(e => e.LimparEventos());

        return eventos;
    }

    private static INotification CriarNotificacao(IDomainEvent evento)
    {
        var tipoNotificacao = typeof(DomainEventNotification<>).MakeGenericType(evento.GetType());
        return (INotification)Activator.CreateInstance(tipoNotificacao, evento)!;
    }
}
