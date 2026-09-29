using GestaoPedidos.Application.Common;
using GestaoPedidos.Domain.Pedidos.Events;
using GestaoPedidos.Domain.Produtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestaoPedidos.Application.Pedidos.EventHandlers;

public class BaixarEstoqueAoConfirmarPedidoHandler(
    IProdutoRepository produtoRepository,
    ILogger<BaixarEstoqueAoConfirmarPedidoHandler> logger)
    : INotificationHandler<DomainEventNotification<PedidoConfirmadoEvent>>
{
    public async Task Handle(DomainEventNotification<PedidoConfirmadoEvent> notification, CancellationToken cancellationToken)
    {
        var evento = notification.Evento;
        var produtos = await produtoRepository.ObterPorIdsAsync(evento.Itens.Select(i => i.ProdutoId), cancellationToken);

        foreach (var item in evento.Itens)
        {
            var produto = produtos.SingleOrDefault(p => p.Id == item.ProdutoId)
                ?? throw new NotFoundException("Produto", item.ProdutoId);

            produto.BaixarEstoque(item.Quantidade);
        }

        logger.LogInformation("Estoque baixado para o pedido {PedidoId} ({QuantidadeItens} itens)", evento.PedidoId, evento.Itens.Count);
    }
}
