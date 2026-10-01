export interface Cliente {
  id: string;
  nome: string;
  email: string;
  documento: string;
}

export type DadosCliente = Omit<Cliente, 'id'>;
