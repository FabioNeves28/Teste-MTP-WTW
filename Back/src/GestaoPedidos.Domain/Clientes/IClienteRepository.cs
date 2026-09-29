namespace GestaoPedidos.Domain.Clientes;

public interface IClienteRepository
{
    Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cliente>> ListarAsync(CancellationToken cancellationToken = default);
    Task<bool> ExisteOutroComMesmoEmailOuDocumentoAsync(Cliente cliente, CancellationToken cancellationToken = default);
    void Adicionar(Cliente cliente);
}
