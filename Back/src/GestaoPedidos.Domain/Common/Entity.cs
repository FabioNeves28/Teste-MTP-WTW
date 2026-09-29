namespace GestaoPedidos.Domain.Common;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public Guid Id { get; protected set; } = Guid.CreateVersion7();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AdicionarEvento(IDomainEvent evento) => _domainEvents.Add(evento);

    public void LimparEventos() => _domainEvents.Clear();
}
