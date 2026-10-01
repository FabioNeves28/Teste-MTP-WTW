using GestaoPedidos.Application.Clientes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestaoPedidos.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<ClienteDto>> Listar(CancellationToken cancellationToken) =>
        sender.Send(new ListarClientesQuery(), cancellationToken);

    [HttpGet("{id:guid}")]
    public Task<ClienteDto> Obter(Guid id, CancellationToken cancellationToken) =>
        sender.Send(new ObterClienteQuery(id), cancellationToken);

    [HttpPost]
    public async Task<ActionResult<ClienteDto>> Criar(CriarClienteCommand command, CancellationToken cancellationToken)
    {
        var cliente = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Obter), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id:guid}")]
    public Task<ClienteDto> Atualizar(Guid id, AtualizarClienteCommand command, CancellationToken cancellationToken) =>
        sender.Send(command with { Id = id }, cancellationToken);
}
