export type StatusPedido = 'Criado' | 'Confirmado' | 'Cancelado' | 'Finalizado';

export const STATUS_PEDIDO: StatusPedido[] = ['Criado', 'Confirmado', 'Cancelado', 'Finalizado'];

export interface ItemPedido {
  produtoId: string;
  nomeProduto: string;
  precoUnitario: number;
  quantidade: number;
  subtotal: number;
}

export interface Pedido {
  id: string;
  clienteId: string;
  clienteNome: string;
  status: StatusPedido;
  criadoEm: string;
  atualizadoEm: string | null;
  total: number;
  itens: ItemPedido[];
}

export interface NovoPedido {
  clienteId: string;
  itens: { produtoId: string; quantidade: number }[];
}

export type AcaoPedido = 'confirmar' | 'cancelar' | 'finalizar';

const ACOES_POR_STATUS: Record<StatusPedido, AcaoPedido[]> = {
  Criado: ['confirmar', 'cancelar'],
  Confirmado: ['finalizar', 'cancelar'],
  Cancelado: [],
  Finalizado: [],
};

export function acoesPermitidas(status: StatusPedido): AcaoPedido[] {
  return ACOES_POR_STATUS[status];
}
