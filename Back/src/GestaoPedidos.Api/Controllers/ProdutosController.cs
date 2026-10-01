using GestaoPedidos.Application.Produtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestaoPedidos.Api.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutosController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<ProdutoDto>> Listar(CancellationToken cancellationToken) =>
        sender.Send(new ListarProdutosQuery(), cancellationToken);

    [HttpGet("{id:guid}")]
    public Task<ProdutoDto> Obter(Guid id, CancellationToken cancellationToken) =>
        sender.Send(new ObterProdutoQuery(id), cancellationToken);

    [HttpPost]
    public async Task<ActionResult<ProdutoDto>> Criar(CriarProdutoCommand command, CancellationToken cancellationToken)
    {
        var produto = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Obter), new { id = produto.Id }, produto);
    }

    [HttpPut("{id:guid}")]
    public Task<ProdutoDto> Atualizar(Guid id, AtualizarProdutoCommand command, CancellationToken cancellationToken) =>
        sender.Send(command with { Id = id }, cancellationToken);
}
