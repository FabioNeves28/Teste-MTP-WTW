import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { aplicarErrosDoServidor } from '../../core/problem-details';
import { ToastService } from '../../core/toast.service';
import { ErroCampoComponent } from '../../shared/erro-campo.component';
import { MoedaPipe } from '../../shared/moeda.pipe';
import { ClientesService } from '../clientes/clientes.service';
import { Produto } from '../produtos/produto.model';
import { ProdutosService } from '../produtos/produtos.service';
import { PedidosService } from './pedidos.service';

interface ItemCarrinho {
  produto: Produto;
  quantidade: number;
}

@Component({
  selector: 'app-pedido-novo',
  imports: [ReactiveFormsModule, RouterLink, ErroCampoComponent, MoedaPipe],
  templateUrl: './pedido-novo.component.html',
})
export class PedidoNovoComponent {
  private readonly pedidosService = inject(PedidosService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
  private readonly fb = inject(NonNullableFormBuilder);

  protected readonly clientes = toSignal(inject(ClientesService).listar(), { initialValue: [] });
  protected readonly produtos = toSignal(inject(ProdutosService).listar(), { initialValue: [] });

  protected readonly form = this.fb.group({
    clienteId: ['', Validators.required],
  });

  protected readonly novoItem = this.fb.group({
    produtoId: ['', Validators.required],
    quantidade: [1, [Validators.required, Validators.min(1)]],
  });

  protected readonly itens = signal<ItemCarrinho[]>([]);

  protected readonly total = computed(() =>
    this.itens().reduce((soma, item) => soma + item.produto.preco * item.quantidade, 0),
  );

  adicionarItem() {
    const { produtoId, quantidade } = this.novoItem.getRawValue();
    const produto = this.produtos().find((p) => p.id === produtoId);

    if (this.novoItem.invalid || !produto) {
      this.novoItem.markAllAsTouched();
      return;
    }

    this.itens.update((itens) => {
      const existente = itens.find((i) => i.produto.id === produtoId);
      return existente
        ? itens.map((i) => (i === existente ? { ...i, quantidade: i.quantidade + quantidade } : i))
        : [...itens, { produto, quantidade }];
    });

    this.novoItem.reset();
  }

  removerItem(produtoId: string) {
    this.itens.update((itens) => itens.filter((i) => i.produto.id !== produtoId));
  }

  salvar() {
    if (this.form.invalid || this.itens().length === 0) {
      this.form.markAllAsTouched();
      this.toast.erro('Selecione o cliente e adicione ao menos um item.');
      return;
    }

    const pedido = {
      clienteId: this.form.getRawValue().clienteId,
      itens: this.itens().map((i) => ({ produtoId: i.produto.id, quantidade: i.quantidade })),
    };

    this.pedidosService.criar(pedido).subscribe({
      next: () => {
        this.toast.sucesso('Pedido criado.');
        this.router.navigate(['/pedidos']);
      },
      error: (erro) => aplicarErrosDoServidor(this.form, erro),
    });
  }
}
