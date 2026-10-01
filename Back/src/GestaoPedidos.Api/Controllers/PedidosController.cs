using GestaoPedidos.Application.Pedidos;
using GestaoPedidos.Domain.Pedidos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestaoPedidos.Api.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidosController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<PedidoDto>> Listar([FromQuery] StatusPedido? status, CancellationToken cancellationToken) =>
        sender.Send(new ListarPedidosQuery(status), cancellationToken);

    [HttpGet("{id:guid}")]
    public Task<PedidoDto> Obter(Guid id, CancellationToken cancellationToken) =>
        sender.Send(new ObterPedidoQuery(id), cancellationToken);

    [HttpPost]
    public async Task<ActionResult<PedidoDto>> Criar(CriarPedidoCommand command, CancellationToken cancellationToken)
    {
        var pedido = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Obter), new { id = pedido.Id }, pedido);
    }

    [HttpPost("{id:guid}/confirmar")]
    public Task<PedidoDto> Confirmar(Guid id, CancellationToken cancellationToken) =>
        sender.Send(new ConfirmarPedidoCommand(id), cancellationToken);

    [HttpPost("{id:guid}/cancelar")]
    public Task<PedidoDto> Cancelar(Guid id, CancellationToken cancellationToken) =>
        sender.Send(new CancelarPedidoCommand(id), cancellationToken);

    [HttpPost("{id:guid}/finalizar")]
    public Task<PedidoDto> Finalizar(Guid id, CancellationToken cancellationToken) =>
        sender.Send(new FinalizarPedidoCommand(id), cancellationToken);
}
