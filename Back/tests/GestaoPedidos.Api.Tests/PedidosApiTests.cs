using System.Net;
using System.Net.Http.Json;
using GestaoPedidos.Application.Clientes;
using GestaoPedidos.Application.Pedidos;
using GestaoPedidos.Application.Produtos;
using Microsoft.AspNetCore.Mvc;

namespace GestaoPedidos.Api.Tests;

public class PedidosApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task FluxoDoPedido_DeveBaixarEstoqueAoConfirmarERestaurarAoCancelar()
    {
        var produto = await CriarProdutoAsync(estoque: 5);
        var cliente = await CriarClienteAsync();

        var pedido = await PostAsync<PedidoDto>("api/pedidos",
            new CriarPedidoCommand(cliente.Id, [new(produto.Id, 3)]));

        pedido.Total.Should().Be(30m);
        (await EstoqueAsync(produto.Id)).Should().Be(5);

        await PostAsync<PedidoDto>($"api/pedidos/{pedido.Id}/confirmar");
        (await EstoqueAsync(produto.Id)).Should().Be(2);

        var cancelado = await PostAsync<PedidoDto>($"api/pedidos/{pedido.Id}/cancelar");
        cancelado.Status.Should().Be("Cancelado");
        (await EstoqueAsync(produto.Id)).Should().Be(5);
    }

    [Fact]
    public async Task CriarPedido_ComQuantidadeMaiorQueEstoque_DeveRetornar422()
    {
        var produto = await CriarProdutoAsync(estoque: 1);
        var cliente = await CriarClienteAsync();

        var resposta = await _client.PostAsJsonAsync("api/pedidos",
            new CriarPedidoCommand(cliente.Id, [new(produto.Id, 2)]));

        resposta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await resposta.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Detail.Should().StartWith("Estoque insuficiente");
    }

    [Fact]
    public async Task CriarPedido_SemItens_DeveRetornar400ComErroPorCampo()
    {
        var resposta = await _client.PostAsJsonAsync("api/pedidos", new CriarPedidoCommand(Guid.NewGuid(), []));

        resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await resposta.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("itens");
    }

    [Fact]
    public async Task ConfirmarPedido_Inexistente_DeveRetornar404()
    {
        var resposta = await _client.PostAsync($"api/pedidos/{Guid.NewGuid()}/confirmar", null);

        resposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private Task<ProdutoDto> CriarProdutoAsync(int estoque) =>
        PostAsync<ProdutoDto>("api/produtos", new CriarProdutoCommand("Caneca", "Caneca 300ml", 10m, estoque));

    private Task<ClienteDto> CriarClienteAsync()
    {
        var documento = Random.Shared.NextInt64(10_000_000_000, 99_999_999_999).ToString();
        return PostAsync<ClienteDto>("api/clientes", new CriarClienteCommand("Cliente Teste", $"{documento}@teste.com", documento));
    }

    private async Task<int> EstoqueAsync(Guid produtoId) =>
        (await _client.GetFromJsonAsync<ProdutoDto>($"api/produtos/{produtoId}"))!.QuantidadeEstoque;

    private async Task<T> PostAsync<T>(string url, object? body = null)
    {
        var resposta = await _client.PostAsJsonAsync(url, body);
        resposta.EnsureSuccessStatusCode();
        return (await resposta.Content.ReadFromJsonAsync<T>())!;
    }
}
