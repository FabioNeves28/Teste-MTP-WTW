using GestaoPedidos.Application.Common;
using GestaoPedidos.Domain.Pedidos.Events;
using GestaoPedidos.Domain.Produtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestaoPedidos.Application.Pedidos.EventHandlers;

public class RestaurarEstoqueAoCancelarPedidoHandler(
    IProdutoRepository produtoRepository,
    ILogger<RestaurarEstoqueAoCancelarPedidoHandler> logger)
    : INotificationHandler<DomainEventNotification<PedidoCanceladoEvent>>
{
    public async Task Handle(DomainEventNotification<PedidoCanceladoEvent> notification, CancellationToken cancellationToken)
    {
        var evento = notification.Evento;
        var produtos = await produtoRepository.ObterPorIdsAsync(evento.Itens.Select(i => i.ProdutoId), cancellationToken);

        foreach (var item in evento.Itens)
        {
            var produto = produtos.SingleOrDefault(p => p.Id == item.ProdutoId)
                ?? throw new NotFoundException("Produto", item.ProdutoId);

            produto.RestaurarEstoque(item.Quantidade);
        }

        logger.LogInformation("Estoque restaurado para o pedido {PedidoId} ({QuantidadeItens} itens)", evento.PedidoId, evento.Itens.Count);
    }
}
