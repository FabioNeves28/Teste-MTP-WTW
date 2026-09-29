namespace GestaoPedidos.Domain.Pedidos.Events;

public sealed record ItemMovimentado(Guid ProdutoId, int Quantidade);
