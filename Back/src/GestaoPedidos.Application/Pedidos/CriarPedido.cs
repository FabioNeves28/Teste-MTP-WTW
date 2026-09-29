using FluentValidation;
using GestaoPedidos.Application.Common;
using GestaoPedidos.Domain.Clientes;
using GestaoPedidos.Domain.Common;
using GestaoPedidos.Domain.Pedidos;
using GestaoPedidos.Domain.Produtos;
using MediatR;

namespace GestaoPedidos.Application.Pedidos;

public record CriarPedidoCommand(Guid ClienteId, IReadOnlyList<NovoItemPedido> Itens) : IRequest<PedidoDto>;

public record NovoItemPedido(Guid ProdutoId, int Quantidade);

public class CriarPedidoValidator : AbstractValidator<CriarPedidoCommand>
{
    public CriarPedidoValidator()
    {
        RuleFor(p => p.ClienteId).NotEmpty();
        RuleFor(p => p.Itens).NotEmpty().WithMessage("Pedido deve ter ao menos um item.");
        RuleForEach(p => p.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.ProdutoId).NotEmpty();
            item.RuleFor(i => i.Quantidade).GreaterThan(0);
        });
    }
}

public class CriarPedidoHandler(
    IPedidoRepository pedidoRepository,
    IClienteRepository clienteRepository,
    IProdutoRepository produtoRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CriarPedidoCommand, PedidoDto>
{
    public async Task<PedidoDto> Handle(CriarPedidoCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.ObterPorIdAsync(request.ClienteId, cancellationToken)
            ?? throw new NotFoundException("Cliente", request.ClienteId);

        var produtos = await produtoRepository.ObterPorIdsAsync(request.Itens.Select(i => i.ProdutoId), cancellationToken);

        var itens = request.Itens.Select(item =>
        {
            var produto = produtos.SingleOrDefault(p => p.Id == item.ProdutoId)
                ?? throw new NotFoundException("Produto", item.ProdutoId);

            return (produto, item.Quantidade);
        });

        var pedido = Pedido.Criar(cliente.Id, itens);

        pedidoRepository.Adicionar(pedido);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return PedidoDto.De(pedido, cliente.Nome);
    }
}
