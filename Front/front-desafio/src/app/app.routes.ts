import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'produtos', pathMatch: 'full' },
  {
    path: 'produtos',
    loadComponent: () =>
      import('./features/produtos/produtos-lista.component').then((m) => m.ProdutosListaComponent),
  },
  {
    path: 'produtos/novo',
    loadComponent: () =>
      import('./features/produtos/produto-form.component').then((m) => m.ProdutoFormComponent),
  },
  {
    path: 'produtos/:id',
    loadComponent: () =>
      import('./features/produtos/produto-form.component').then((m) => m.ProdutoFormComponent),
  },
  {
    path: 'clientes',
    loadComponent: () =>
      import('./features/clientes/clientes-lista.component').then((m) => m.ClientesListaComponent),
  },
  {
    path: 'clientes/novo',
    loadComponent: () =>
      import('./features/clientes/cliente-form.component').then((m) => m.ClienteFormComponent),
  },
  {
    path: 'clientes/:id',
    loadComponent: () =>
      import('./features/clientes/cliente-form.component').then((m) => m.ClienteFormComponent),
  },
  {
    path: 'pedidos',
    loadComponent: () =>
      import('./features/pedidos/pedidos-lista.component').then((m) => m.PedidosListaComponent),
  },
  {
    path: 'pedidos/novo',
    loadComponent: () =>
      import('./features/pedidos/pedido-novo.component').then((m) => m.PedidoNovoComponent),
  },
  { path: '**', redirectTo: 'produtos' },
];
