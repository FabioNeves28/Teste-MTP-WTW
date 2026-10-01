export interface Produto {
  id: string;
  nome: string;
  descricao: string;
  preco: number;
  quantidadeEstoque: number;
}

export type DadosProduto = Omit<Produto, 'id'>;
