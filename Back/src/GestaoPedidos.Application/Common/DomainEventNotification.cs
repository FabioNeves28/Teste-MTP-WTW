using GestaoPedidos.Domain.Common;
using MediatR;

namespace GestaoPedidos.Application.Common;

public sealed record DomainEventNotification<TEvent>(TEvent Evento) : INotification
    where TEvent : IDomainEvent;
